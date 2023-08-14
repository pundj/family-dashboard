using FamilyDashboard.Blazor.Models.SmartHome;
using System.Net.Http.Headers;
using System.Text.Json;

namespace FamilyDashboard.Blazor.Services;

public class SmartThingsService : ISmartHomeService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<SmartThingsService> _logger;

    public SmartThingsService(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<SmartThingsService> logger)
    {
        _httpClient = httpClientFactory.CreateClient("SmartHome");
        var baseAddress = configuration.GetValue<string>("SmartThingsApiBaseAddress") ?? throw new Exception("SmartThingsApiBaseAddress app setting is null. Please set the value in the appsettings.json");
        _httpClient.BaseAddress = new Uri(baseAddress);
        var accessToken = configuration.GetValue<string>("SmartThingsApiAccessToken") ?? throw new Exception("SmartThingsApiAccessToken app setting is null. Please set the value in the appsettings.json");
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        _logger = logger;
    }

    public async Task<GetSmartHomeDevicesResponse?> GetDevicesAsync()
    {
        var httpResponse = await _httpClient.GetAsync($"devices");
        await LogResponseAsync(httpResponse);
        var responseBody = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);

        var devicesResponse = GetDevicesFromJsonResponseBody(responseBody);

        foreach (var response in devicesResponse.Items ?? new List<SmartThingsDeviceViewModel>())
        {
            try
            {
                httpResponse = await _httpClient.GetAsync($"devices/{response.DeviceId}/status");
                await LogResponseAsync(httpResponse);
                responseBody = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                GetDeviceFromJsonResponseBody(responseBody, response);
                httpResponse = await _httpClient.GetAsync($"devices/{response.DeviceId}/health");
                await LogResponseAsync(httpResponse);
                responseBody = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                GetDeviceOnlineStateFromJsonResponseBody(responseBody, response);
            }
            catch (Exception ex)
            {
                throw new Exception($"Device Id {response.DeviceId} {ex.Message}", ex);
            }
        }
        devicesResponse.Items = devicesResponse.Items?.Where(d => d.Type != SmartHomeDeviceType.Hub).OrderByDescending(d => d.OnlineState).ThenBy(x => x.Label).ToList();

        return devicesResponse;
    }

    public async Task<SmartThingsDeviceViewModel?> GetDeviceAsync(string? deviceId)
    {
        if (deviceId == null)
            throw new ArgumentNullException(nameof(deviceId));

        var httpResponse = await _httpClient.GetAsync($"devices/{deviceId}");
        await LogResponseAsync(httpResponse);
        var responseBody = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        var deviceResponse = GetDeviceFromJsonResponseBody(responseBody);

        httpResponse = await _httpClient.GetAsync($"devices/{deviceId}/status");
        await LogResponseAsync(httpResponse);
        responseBody = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        GetDeviceFromJsonResponseBody(responseBody, deviceResponse);

        httpResponse = await _httpClient.GetAsync($"devices/{deviceId}/health");
        await LogResponseAsync(httpResponse);
        responseBody = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        GetDeviceOnlineStateFromJsonResponseBody(responseBody, deviceResponse);

        return deviceResponse;
    }

    public async Task SetSwitchAsync(string? deviceId, SmartHomeSwitch switchValue)
    {
        if (deviceId == null)
            throw new ArgumentNullException(nameof(deviceId));
        if (switchValue == SmartHomeSwitch.Unknown)
            throw new ArgumentException("switchValue cannot be Unknown", nameof(switchValue));

        var commands = new List<string>()
            {
                $@"{{""component"": ""main"",""capability"": ""switch"",""command"": ""{switchValue.ToString().ToLower()}""}}",
            };
        var postData = $@"{{""commands"": [{string.Join(",", commands)}]}}";
        var body = new StringContent(postData);
        var httpResponse = await _httpClient.PostAsync($"devices/{deviceId}/commands", body);
        await LogResponseAsync(httpResponse);
    }

    private async Task LogResponseAsync(HttpResponseMessage response)
    {
        var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError($"Error trying to call SmartThings API...  Response body: {responseBody}");
            response.EnsureSuccessStatusCode();
        }
        else
        {
            _logger.LogDebug($"Successfully called SmartThings API...  Response body: {responseBody}");
        }
    }

    private static GetSmartHomeDevicesResponse GetDevicesFromJsonResponseBody(string json)
    {
        using var doc = JsonDocument.Parse(json);
        var response = new GetSmartHomeDevicesResponse();
        if (!doc.RootElement.TryGetProperty("items", out var items))
            throw new ArgumentException("Json response does not have expected field \"items\"", nameof(json));

        var responseItems = new List<SmartThingsDeviceViewModel>();

        foreach (var device in items.EnumerateArray())
        {
            if (!device.TryGetProperty("deviceId", out var deviceId))
                throw new ArgumentException("Json response does not have expected field \"deviceId\"", nameof(json));
            if (!device.TryGetProperty("label", out var label))
                throw new ArgumentException("Json response does not have expected field \"label\"", nameof(json));
            if (!device.TryGetProperty("components", out var components))
                throw new ArgumentException("Json response does not have expected field \"components\"", nameof(json));
            var component = components.EnumerateArray().FirstOrDefault();
            if (!component.TryGetProperty("categories", out var categories))
                throw new ArgumentException("Json response does not have expected field \"categories\" under \"components\"", nameof(json));
            var category = categories.EnumerateArray().FirstOrDefault();
            if (!category.TryGetProperty("name", out var deviceType))
                throw new ArgumentException("Json response does not have expected field \"name\" under \"categories\"", nameof(json));

            var deviceResponse = new SmartThingsDeviceViewModel
            {
                DeviceId = deviceId.GetString(),
                Label = label.GetString(),
                Type = Enum.Parse<SmartHomeDeviceType>(deviceType.GetString() ?? "", ignoreCase: true)
            };

            responseItems.Add(deviceResponse);
        }
        if (responseItems.Any())
            response.Items = responseItems;

        return response;
    }

    private static SmartThingsDeviceViewModel GetDeviceFromJsonResponseBody(string json, SmartThingsDeviceViewModel? response = null)
    {
        response ??= new SmartThingsDeviceViewModel();
        if (response.Type == SmartHomeDeviceType.Hub)
            return response;

        using var doc = JsonDocument.Parse(json);

        if (!doc.RootElement.TryGetProperty("components", out var components))
            throw new ArgumentException("Json response does not have expected field \"components\"", nameof(json));

        if (response.DeviceId is null)
        {
            if (!doc.RootElement.TryGetProperty("deviceId", out var deviceId))
                throw new ArgumentException("Json response does not have expected field \"deviceId\"", nameof(json));
            response.DeviceId = deviceId.GetString();
        }

        if (response.Type == SmartHomeDeviceType.Unknown)
        {
            var component = components.EnumerateArray().FirstOrDefault();
            if (!component.TryGetProperty("categories", out var categories))
                throw new ArgumentException("Json response does not have expected field \"categories\" under \"components\"", nameof(json));
            var category = categories.EnumerateArray().FirstOrDefault();
            if (!category.TryGetProperty("name", out var deviceType))
                throw new ArgumentException("Json response does not have expected field \"name\" under \"categories\"", nameof(json));
            response.Type = Enum.Parse<SmartHomeDeviceType>(deviceType.GetString() ?? "", ignoreCase: true);
        }

        if (response.Label is null)
        {
            if (!doc.RootElement.TryGetProperty("label", out var label))
                throw new ArgumentException("Json response does not have expected field \"label\"", nameof(json));
            response.Label = label.GetString();
            return response;
        }

        if (!components.TryGetProperty("main", out var main))
            throw new ArgumentException($"Json response for device {response.DeviceId} - {response.Label} does not have expected field \"main\"", nameof(json));

        if (main.TryGetProperty("switch", out var switchHeader) &&
            switchHeader.TryGetProperty("switch", out var switchDetail) &&
            switchDetail.TryGetProperty("value", out var switchValue))
        {
            _ = Enum.TryParse<SmartHomeSwitch>(switchValue.ToString(), ignoreCase: true, out var smartHomeSwitch);
            response.SwitchValue = smartHomeSwitch;
        }
        
        if (main.TryGetProperty("alarm", out var alarmHeader) &&
            alarmHeader.TryGetProperty("alarm", out var alarmDetail) &&
            alarmDetail.TryGetProperty("value", out var alarmValue))
        {
            _ = Enum.TryParse<SmartHomeSwitch>(alarmValue.ToString(), ignoreCase: true, out var alarmSwitch);
            response.AlarmValue = alarmSwitch;
        }

        if (main.TryGetProperty("colorControl", out var colorControl))
        {
            if (colorControl.TryGetProperty("hue", out var hue) &&
                hue.TryGetProperty("value", out var hueValue))
                response.Hue = hueValue.GetInt32();
            if (colorControl.TryGetProperty("saturation", out var saturation) &&
                saturation.TryGetProperty("value", out var saturationValue))
                response.Saturation = saturationValue.GetInt32();
        }

        if (main.TryGetProperty("colorTemperature", out var colorTemperatureHeader) &&
            colorTemperatureHeader.TryGetProperty("colorTemperature", out var colorTemperatureDetail) &&
            colorTemperatureDetail.TryGetProperty("value", out var colorTemperatureValue))
            response.Temperature = colorTemperatureValue.GetInt32();

        if (main.TryGetProperty("switchLevel", out var switchLevel) &&
            switchLevel.TryGetProperty("level", out var level) &&
            level.TryGetProperty("value", out var levelValue))
            response.Level = levelValue.GetInt32();

        if (main.TryGetProperty("smokeDetector", out var smokeDetector) &&
            smokeDetector.TryGetProperty("smoke", out var smoke) &&
            smoke.TryGetProperty("value", out var smokeValue))
        {
            _ = Enum.TryParse<SmartHomeSmokeCODetectorState>(smokeValue.ToString(), ignoreCase: true, out var smartHomeSmokeDetectorState);
            response.SmokeDetectorState = smartHomeSmokeDetectorState;
        }

        if (main.TryGetProperty("carbonMonoxideDetector", out var carbonMonoxideDetector) &&
            carbonMonoxideDetector.TryGetProperty("carbonMonoxide", out var carbonMonoxide) &&
            carbonMonoxide.TryGetProperty("value", out var carbonMonoxideValue))
        {
            _ = Enum.TryParse<SmartHomeSmokeCODetectorState>(carbonMonoxideValue.ToString(), ignoreCase: true, out var smartHomeCarbonMonoxideDetectorState);
            response.CarbonMonoxideDetectorState = smartHomeCarbonMonoxideDetectorState;
        }

        if (main.TryGetProperty("battery", out var battery) &&
            battery.TryGetProperty("battery", out var batterySecondTier) &&
            batterySecondTier.TryGetProperty("value", out var batteryValue) &&
            batterySecondTier.TryGetProperty("unit", out var batteryUnit))
            response.BatteryLevel = $"{batteryValue.GetInt32()}{batteryUnit.GetString()}";

        if (main.TryGetProperty("relativeHumidityMeasurement", out var relativeHumidityMeasurement) &&
            relativeHumidityMeasurement.TryGetProperty("humidity", out var humidity) &&
            humidity.TryGetProperty("value", out var humidityValue) &&
            humidity.TryGetProperty("unit", out var humidityUnit))
            response.ThermostatHumidityMeasurement = $"{humidityValue.GetInt32()}{humidityUnit.GetString()}";

        if (main.TryGetProperty("temperatureMeasurement", out var temperatureMeasurement) &&
            temperatureMeasurement.TryGetProperty("temperature", out var temperature) &&
            temperature.TryGetProperty("value", out var temperatureValue) &&
            temperature.TryGetProperty("unit", out var temperatureUnit))
            response.TemperatureMeasurement = $"{temperatureValue.GetDecimal()}°{temperatureUnit.GetString()}";

        if (main.TryGetProperty("thermostatHeatingSetpoint", out var thermostatHeatingSetpoint) &&
            thermostatHeatingSetpoint.TryGetProperty("heatingSetpoint", out var heatingSetpoint) &&
            heatingSetpoint.TryGetProperty("value", out var heatingSetpointValue) &&
            heatingSetpoint.TryGetProperty("unit", out var heatingSetpointUnit))
            response.ThermostatHeatingSetpoint = $"{heatingSetpointValue.GetInt32()}°{heatingSetpointUnit.GetString()}";

        if (main.TryGetProperty("thermostatCoolingSetpoint", out var thermostatCoolingSetpoint) &&
            thermostatCoolingSetpoint.TryGetProperty("coolingSetpoint", out var coolingSetpoint) &&
            coolingSetpoint.TryGetProperty("value", out var coolingSetpointValue) &&
            coolingSetpoint.TryGetProperty("unit", out var coolingSetpointUnit))
            response.ThermostatCoolingSetpoint = $"{coolingSetpointValue.GetInt32()}°{coolingSetpointUnit.GetString()}";

        if (main.TryGetProperty("thermostatMode", out var thermostatMode) &&
            thermostatMode.TryGetProperty("value", out var thermostatModeValue))
        {
            _ = Enum.TryParse<SmartHomeThermostatMode>(thermostatModeValue.ToString(), ignoreCase: true, out var smartHomeThermostatMode);
            response.ThermostatMode = smartHomeThermostatMode;
        }

        if (main.TryGetProperty("thermostatOperatingState", out var thermostatOperatingState) &&
            thermostatOperatingState.TryGetProperty("thermostatOperatingState", out var thermostatOperatingStateDetail) &&
            thermostatOperatingStateDetail.TryGetProperty("value", out var thermostatOperatingStateValue))
        {
            _ = Enum.TryParse<SmartHomeThermostatOperatingState>(thermostatOperatingStateValue.ToString(), ignoreCase: true, out var smartHomeThermostatOperatingValue);
            response.ThermostatOperatingState = smartHomeThermostatOperatingValue;
        }

        if (main.TryGetProperty("valve", out var valve) &&
            valve.TryGetProperty("valve", out var valveDetail) &&
            valveDetail.TryGetProperty("value", out var valveState))
        {
            _ = Enum.TryParse<SmartHomeOpenState>(valveState.ToString(), ignoreCase: true, out var smartHomeOpenState);
            response.ValveState = smartHomeOpenState;
        }

        if (main.TryGetProperty("contactSensor", out var contactSensor) &&
            contactSensor.TryGetProperty("contact", out var contact) &&
            contact.TryGetProperty("value", out var contactValue))
        {
            _ = Enum.TryParse<SmartHomeOpenState>(contactValue.ToString(), ignoreCase: true, out var smartHomeOpenState);
            response.ContactState = smartHomeOpenState;
        }

        if (main.TryGetProperty("presenceSensor", out var presenceSensor) &&
            presenceSensor.TryGetProperty("presence", out var presence) &&
            presence.TryGetProperty("value", out var presenceValue))
        {
            _ = Enum.TryParse<SmartHomePresenceState>(presenceValue.ToString().Replace(" ",""), ignoreCase: true, out var smartHomePresenceState);
            response.PresenceState = smartHomePresenceState;
        }

        if (main.TryGetProperty("waterSensor", out var waterSensor) &&
            waterSensor.TryGetProperty("water", out var water) &&
            water.TryGetProperty("value", out var waterValue))
        {
            _ = Enum.TryParse<SmartHomeWaterSensorState>(waterValue.ToString(), ignoreCase: true, out var smartHomeWaterSensorState);
            response.WaterSensorState = smartHomeWaterSensorState;
        }

        return response;
    }

    private static void GetDeviceOnlineStateFromJsonResponseBody(string json, SmartThingsDeviceViewModel response)
    {
        using var doc = JsonDocument.Parse(json);

        if (!doc.RootElement.TryGetProperty("state", out var state))
            throw new ArgumentException("Json response does not have expected field \"state\"", nameof(json));

        _ = Enum.TryParse<SmartHomeOnlineState>(state.ToString(), ignoreCase: true, out var smartHomeOnlineState);
        response.OnlineState = smartHomeOnlineState;
    }
}