# DeployToAzureAppService.ps1
param(
  [Parameter(Mandatory = $true)]
  [string]$SubscriptionId,

  [string]$TenantId,

  [Parameter(Mandatory = $true)]
  [string]$WebAppName,

  [string]$Location = "centralus",
  [string]$ResourceGroup = "rg-family-dashboard",
  [string]$PlanName = "asp-family-dashboard",
  [string]$Sku = "B1",
  [string]$AspNetCoreEnvironment = "Production",
  [string]$Runtime = "DOTNETCORE|10.0",

  # API appsettings
  [string]$ApiConnectionString = "Data Source=App_Data/familydashboard.db",
  [string]$SmartThingsBaseAddress = "https://api.smartthings.com/v1/",
  [string[]]$AdditionalApiAppSettings = @(),
  [string[]]$AllowedIpAddresses = @(),

  # Blazor wwwroot/appsettings.json values (optional)
  [string]$FamilyName,
  [double]$WeatherLatitude,
  [double]$WeatherLongitude,
  [string]$WeatherLocationName,
  [string]$CameraViewerUrl,
  [string]$GoogleClientId,
  [string]$GoogleClientSecret,
  [string]$GoogleRedirectUri,
  [string[]]$GoogleCalendarIds = @(),
  [string]$GoogleCalendarNamesJson
)

$ErrorActionPreference = "Stop"

$script:InputParameters = @{}
foreach ($kvp in $PSBoundParameters.GetEnumerator()) {
  $script:InputParameters[$kvp.Key] = $kvp.Value
}

$script:BlazorAppSettingsPath = Join-Path $PSScriptRoot "FamilyDashboard.Blazor\wwwroot\appsettings.json"
$script:OriginalBlazorAppSettingsContent = $null
$script:OriginalBlazorAppSettingsBytes = $null
$script:RestoreBlazorAppSettings = $false

function Resolve-AppServiceRuntime {
  param(
    [string]$Runtime
  )

  if (-not [string]::IsNullOrWhiteSpace($Runtime)) {
    return $Runtime
  }

  $availableRuntimes = @(& az webapp list-runtimes --os linux --output tsv)
  if ($LASTEXITCODE -ne 0) {
    throw "Failed to list Linux web app runtimes."
  }

  $preferred = @(
    "DOTNETCORE|10.0",
    "DOTNETCORE|10"
  )

  foreach ($candidate in $preferred) {
    if ($availableRuntimes -contains $candidate) {
      return $candidate
    }

    $matchedRuntime = $availableRuntimes | Where-Object { $_.ToLowerInvariant() -eq $candidate.ToLowerInvariant() } | Select-Object -First 1
    if (-not [string]::IsNullOrWhiteSpace($matchedRuntime)) {
      return $matchedRuntime
    }
  }

  throw "No .NET 10 runtime found for App Service Linux. Re-run with -Runtime explicitly after checking: az webapp list-runtimes --os linux -o tsv"
}

function Ensure-AzContext {
  param(
    [string]$SubscriptionId,
    [string]$TenantId
  )

  $accountInfo = & az account show --output json 2>$null
  if ($LASTEXITCODE -ne 0 -or [string]::IsNullOrWhiteSpace($accountInfo)) {
    Write-Host "No active Azure CLI login detected. Logging in..."
    if ([string]::IsNullOrWhiteSpace($TenantId)) {
      az login
      if ($LASTEXITCODE -ne 0) { throw "Azure login failed." }
    }
    else {
      az login --tenant $TenantId
      if ($LASTEXITCODE -ne 0) { throw "Azure tenant login failed for tenant '$TenantId'." }
    }
  }

  az account set --subscription $SubscriptionId
  if ($LASTEXITCODE -ne 0) {
    if ([string]::IsNullOrWhiteSpace($TenantId)) {
      throw "Unable to access subscription '$SubscriptionId' with current Azure CLI login. Run 'az login' with the correct tenant or pass -TenantId."
    }

    Write-Host "Current Azure CLI context cannot access subscription '$SubscriptionId'. Re-authenticating with tenant '$TenantId'..."
    az login --tenant $TenantId
    if ($LASTEXITCODE -ne 0) { throw "Azure tenant login failed for tenant '$TenantId'." }

    az account set --subscription $SubscriptionId
    if ($LASTEXITCODE -ne 0) { throw "Unable to select subscription '$SubscriptionId' after tenant login." }
  }
}

function Update-BlazorAppSettings {
  if (-not (Test-Path $script:BlazorAppSettingsPath)) {
    throw "Could not find $script:BlazorAppSettingsPath"
  }

  $script:OriginalBlazorAppSettingsBytes = Get-Content -Path $script:BlazorAppSettingsPath -AsByteStream -Raw
  $script:OriginalBlazorAppSettingsContent = [System.Text.Encoding]::UTF8.GetString($script:OriginalBlazorAppSettingsBytes)
  $script:RestoreBlazorAppSettings = $true

  $settings = $script:OriginalBlazorAppSettingsContent | ConvertFrom-Json -AsHashtable

  if (-not $settings.ContainsKey("GoogleOAuth") -or $null -eq $settings["GoogleOAuth"]) {
    $settings["GoogleOAuth"] = @{}
  }

  if (-not $settings.ContainsKey("Weather") -or $null -eq $settings["Weather"]) {
    $settings["Weather"] = @{}
  }

  if ($script:InputParameters.ContainsKey("FamilyName")) { $settings["FamilyName"] = $FamilyName }
  if ($script:InputParameters.ContainsKey("WeatherLatitude")) { $settings["Weather"]["Latitude"] = $WeatherLatitude }
  if ($script:InputParameters.ContainsKey("WeatherLongitude")) { $settings["Weather"]["Longitude"] = $WeatherLongitude }
  if ($script:InputParameters.ContainsKey("WeatherLocationName")) { $settings["Weather"]["LocationName"] = $WeatherLocationName }
 if ($script:InputParameters.ContainsKey("CameraViewerUrl")) {
  if ([string]::IsNullOrWhiteSpace($CameraViewerUrl)) {
    $settings["CameraViewerUrl"] = $null
  }
  else {
    $trimmedCameraViewerUrl = $CameraViewerUrl.Trim()
    $cameraViewerUri = $null
    if (-not [Uri]::TryCreate($trimmedCameraViewerUrl, [UriKind]::Absolute, [ref]$cameraViewerUri) -or
        $cameraViewerUri.Scheme -notin @([Uri]::UriSchemeHttp, [Uri]::UriSchemeHttps)) {
      throw "CameraViewerUrl must be an absolute http/https URL (e.g. https://my.wyze.com/home)."
    }

    $settings["CameraViewerUrl"] = $cameraViewerUri.ToString()
  }
}
  if ($script:InputParameters.ContainsKey("GoogleClientId")) { $settings["GoogleOAuth"]["ClientId"] = $GoogleClientId }
  if ($script:InputParameters.ContainsKey("GoogleClientSecret")) { $settings["GoogleOAuth"]["ClientSecret"] = $GoogleClientSecret }
  if ($script:InputParameters.ContainsKey("GoogleRedirectUri")) { $settings["GoogleOAuth"]["RedirectUri"] = $GoogleRedirectUri }

  if ($GoogleCalendarIds.Count -gt 0) {
    $settings["GoogleOAuth"]["CalendarIds"] = $GoogleCalendarIds
  }

  if (-not [string]::IsNullOrWhiteSpace($GoogleCalendarNamesJson)) {
    $settings["GoogleOAuth"]["CalendarNames"] = $GoogleCalendarNamesJson | ConvertFrom-Json -AsHashtable
  }

  $settings | ConvertTo-Json -Depth 12 | Set-Content -Path $script:BlazorAppSettingsPath -Encoding UTF8
}

function Restore-BlazorAppSettingsIfNeeded {
  if ($script:RestoreBlazorAppSettings) {
    if ($null -ne $script:OriginalBlazorAppSettingsBytes) {
      [System.IO.File]::WriteAllBytes($script:BlazorAppSettingsPath, $script:OriginalBlazorAppSettingsBytes)
    }
    elseif ($null -ne $script:OriginalBlazorAppSettingsContent) {
      Set-Content -Path $script:BlazorAppSettingsPath -Value $script:OriginalBlazorAppSettingsContent -Encoding UTF8 -NoNewline
    }

    $script:RestoreBlazorAppSettings = $false
    $script:OriginalBlazorAppSettingsContent = $null
    $script:OriginalBlazorAppSettingsBytes = $null
  }
}

function Normalize-AllowedIpAddresses {
  param([string[]]$IpAddresses)

  $normalized = @()
  foreach ($ip in $IpAddresses) {
    if ([string]::IsNullOrWhiteSpace($ip)) { continue }

    $trimmed = $ip.Trim()
    if ($trimmed.Contains("/")) {
      $normalized += $trimmed
    }
    else {
      $normalized += "$trimmed/32"
    }
  }

  return @($normalized | Select-Object -Unique)
}

function Apply-IpAccessRestrictions {
  param(
    [string]$ResourceGroup,
    [string]$WebAppName,
    [string[]]$IpAddresses,
    [string]$SubscriptionId
  )

  $normalizedIpAddresses = Normalize-AllowedIpAddresses -IpAddresses $IpAddresses
  if ($normalizedIpAddresses.Count -eq 0) {
    Write-Host "No AllowedIpAddresses provided. Skipping access restriction changes."
    return
  }

  Write-Host "Applying IP restrictions for $($normalizedIpAddresses.Count) address(es)..."

  # Remove previously managed rules to keep reruns idempotent
  $restrictionStateJson = & az webapp config access-restriction show --resource-group $ResourceGroup --name $WebAppName --output json
  if ($LASTEXITCODE -ne 0) { throw "Failed to read existing access restrictions for '$WebAppName'." }

  $restrictionState = $restrictionStateJson | ConvertFrom-Json
  $managedRuleNames = @()

  foreach ($rule in @($restrictionState.ipSecurityRestrictions)) {
    if ($null -ne $rule.name -and $rule.name.StartsWith("DeployScript-Allow-")) {
      $managedRuleNames += $rule.name
    }
  }

  foreach ($rule in @($restrictionState.scmIpSecurityRestrictions)) {
    if ($null -ne $rule.name -and $rule.name.StartsWith("DeployScript-Allow-")) {
      $managedRuleNames += $rule.name
    }
  }

  foreach ($ruleNameToRemove in ($managedRuleNames | Select-Object -Unique)) {
    if ($ruleNameToRemove.EndsWith("-SCM")) {
      az webapp config access-restriction remove `
        --resource-group $ResourceGroup `
        --name $WebAppName `
        --scm-site true `
        --rule-name $ruleNameToRemove
    }
    else {
      az webapp config access-restriction remove `
        --resource-group $ResourceGroup `
        --name $WebAppName `
        --rule-name $ruleNameToRemove
    }

    if ($LASTEXITCODE -ne 0) { throw "Failed to remove existing managed access rule '$ruleNameToRemove'." }
  }

  # Add allow rules for main site and SCM/Kudu site
  for ($i = 0; $i -lt $normalizedIpAddresses.Count; $i++) {
    $priority = 100 + $i
    $ruleIndex = $i + 1
    $ruleName = "DeployScript-Allow-$ruleIndex"
    $ipCidr = $normalizedIpAddresses[$i]

    az webapp config access-restriction add `
      --resource-group $ResourceGroup `
      --name $WebAppName `
      --rule-name $ruleName `
      --action Allow `
      --ip-address $ipCidr `
      --priority $priority
    if ($LASTEXITCODE -ne 0) { throw "Failed to add main-site access rule '$ruleName' for $ipCidr." }

    az webapp config access-restriction add `
      --resource-group $ResourceGroup `
      --name $WebAppName `
      --scm-site true `
      --rule-name "$ruleName-SCM" `
      --action Allow `
      --ip-address $ipCidr `
      --priority $priority
    if ($LASTEXITCODE -ne 0) { throw "Failed to add SCM access rule '$ruleName-SCM' for $ipCidr." }
  }

  # Set both main-site and SCM defaults to deny
  $configResourceId = "/subscriptions/$SubscriptionId/resourceGroups/$ResourceGroup/providers/Microsoft.Web/sites/$WebAppName/config/web"
  az resource update `
    --ids $configResourceId `
    --set properties.ipSecurityRestrictionsDefaultAction=Deny properties.scmIpSecurityRestrictionsDefaultAction=Deny
  if ($LASTEXITCODE -ne 0) { throw "Failed to set default access restriction action to Deny for app '$WebAppName'." }
}

try {
  # 1) Update client appsettings before publish
  Update-BlazorAppSettings

  # 2) Azure context/subscription
  Ensure-AzContext -SubscriptionId $SubscriptionId -TenantId $TenantId

  # 3) Infra
  az group create --name $ResourceGroup --location $Location
  if ($LASTEXITCODE -ne 0) { throw "Failed to create or validate resource group '$ResourceGroup'." }

  az appservice plan create `
      --name $PlanName `
      --resource-group $ResourceGroup `
      --is-linux `
      --sku $Sku
  if ($LASTEXITCODE -ne 0) { throw "Failed to create or validate App Service plan '$PlanName'." }

  $effectiveRuntime = Resolve-AppServiceRuntime -Runtime $Runtime
  Write-Host "Using App Service runtime: $effectiveRuntime"

  # Quote runtime explicitly to avoid shell parsing issues with values like dotnet|10
  $runtimeForAz = "`"$effectiveRuntime`""

  az webapp create `
    --name $WebAppName `
    --resource-group $ResourceGroup `
    --plan $PlanName `
    --runtime $runtimeForAz
  if ($LASTEXITCODE -ne 0) { throw "Failed to create or validate web app '$WebAppName'." }

  # Verify web app exists before deploy/configure
  az webapp show `
    --name $WebAppName `
    --resource-group $ResourceGroup
  if ($LASTEXITCODE -ne 0) { throw "Web app '$WebAppName' was not found after creation." }

  # 4) Build/publish API (hosted Blazor app entrypoint)
  dotnet publish .\FamilyDashboard.Api\FamilyDashboard.Api.csproj -c Release -o .\artifacts\publish
  if ($LASTEXITCODE -ne 0) { throw "dotnet publish failed." }

  # 5) Zip deploy
  if (Test-Path .\artifacts\publish.zip) { Remove-Item .\artifacts\publish.zip -Force }
  Compress-Archive -Path .\artifacts\publish\* -DestinationPath .\artifacts\publish.zip -Force
  az webapp deploy `
    --resource-group $ResourceGroup `
    --name $WebAppName `
    --src-path .\artifacts\publish.zip `
    --type zip
  if ($LASTEXITCODE -ne 0) { throw "Web app deployment failed for '$WebAppName'." }

  # 6) Ensure Linux startup command points to the deployed API assembly
  az webapp config set `
    --resource-group $ResourceGroup `
    --name $WebAppName `
    --startup-file "dotnet FamilyDashboard.Api.dll"
  if ($LASTEXITCODE -ne 0) { throw "Failed to set startup command for '$WebAppName'." }

  # 7) API app settings from script parameters
  $apiSettings = @(
    "ASPNETCORE_ENVIRONMENT=$AspNetCoreEnvironment",
    "ConnectionStrings__DefaultConnection=$ApiConnectionString",
    "SmartThings__BaseAddress=$SmartThingsBaseAddress"
  )

  if ($AdditionalApiAppSettings.Count -gt 0) {
    $apiSettings += $AdditionalApiAppSettings
  }

  az webapp config appsettings set `
    --resource-group $ResourceGroup `
    --name $WebAppName `
    --settings $apiSettings
  if ($LASTEXITCODE -ne 0) { throw "Failed to apply app settings for '$WebAppName'." }

  # Optional: ensure not using read-only run-from-package for SQLite writes
  az webapp config appsettings delete `
    --resource-group $ResourceGroup `
    --name $WebAppName `
    --setting-names WEBSITE_RUN_FROM_PACKAGE
  if ($LASTEXITCODE -ne 0) { throw "Failed to remove WEBSITE_RUN_FROM_PACKAGE setting for '$WebAppName'." }

  # 8) Optional: lock down app access by IP allow-list
  Apply-IpAccessRestrictions `
    -ResourceGroup $ResourceGroup `
    -WebAppName $WebAppName `
    -IpAddresses $AllowedIpAddresses `
    -SubscriptionId $SubscriptionId

  Write-Host "Deployed to: https://$WebAppName.azurewebsites.net"
}
finally {
  Restore-BlazorAppSettingsIfNeeded
}