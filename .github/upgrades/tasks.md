# .NET 10 Upgrade - Execution Tasks

## Task Overview

**Scenario**: Upgrade FamilyDashboard from .NET 7.0 to .NET 10.0
**Strategy**: All-At-Once (Single atomic upgrade)
**Total Tasks**: 6
**Branch**: upgrade-to-NET10

---

## Tasks

### [?] TASK-001: Verify Prerequisites *(Completed: 2026-02-05 13:35)*
**Description**: Verify that all required tools and environment setup is complete before starting the upgrade

**Actions**:
- [?] (1) Verify .NET 10 SDK is installed on the machine
  - Run: `dotnet --list-sdks`
  - Expected: .NET 10.x SDK listed in output
- [?] (2) Verify current branch is `upgrade-to-NET10`
  - Run: `git branch --show-current`
  - Expected: Output shows "upgrade-to-NET10"
- [?] (3) Verify no pending uncommitted changes
  - Run: `git status`
  - Expected: Working tree clean or only expected changes present

**Validation**: All prerequisites confirmed

**References**: Plan §2.1 Prerequisites

---

### [?] TASK-002: Update Target Framework *(Completed: 2026-02-05 13:36)*
**Description**: Update the project file to target .NET 10.0 framework

**Actions**:
- [?] (1) Open file: `FamilyDashboard.Blazor\FamilyDashboard.Blazor.csproj`
- [?] (2) Locate the `<TargetFramework>` element
- [?] (3) Change `<TargetFramework>net7.0</TargetFramework>` to `<TargetFramework>net10.0</TargetFramework>`
- [?] (4) Save the file
- [?] (5) Verify change applied correctly

**Validation**: Project file contains `<TargetFramework>net10.0</TargetFramework>`

**References**: Plan §4.1.2 Technology/Framework Update

---

### [?] TASK-003: Update NuGet Package References *(Completed: 2026-02-05 13:37)*
**Description**: Update all NuGet packages to .NET 10 compatible versions

**Actions**:
- [?] (1) Open file: `FamilyDashboard.Blazor\FamilyDashboard.Blazor.csproj`
- [?] (2) Update package: Microsoft.AspNetCore.Components.WebAssembly
  - From: Version="7.0.5"
  - To: Version="10.0.2"
- [?] (3) Update package: Microsoft.AspNetCore.Components.WebAssembly.DevServer
  - From: Version="7.0.5"
  - To: Version="10.0.2"
- [?] (4) Update package: Microsoft.Extensions.Http
  - From: Version="7.0.0"
  - To: Version="10.0.2"
- [?] (5) Save the file
- [?] (6) Restore packages: `dotnet restore FamilyDashboard.Blazor/FamilyDashboard.Blazor.csproj`
- [?] (7) Verify all packages restored successfully

**Validation**: All three packages updated to version 10.0.2 and restored without errors

**References**: Plan §4.1.3 Package Updates

---

### [?] TASK-004: Build and Identify Compilation Errors *(Completed: 2026-02-05 13:39)*
**Description**: Attempt initial build to identify API compatibility issues that need fixing

**Actions**:
- [?] (1) Clean solution: `dotnet clean FamilyDashboard.sln`
- [?] (2) Build project: `dotnet build FamilyDashboard.Blazor/FamilyDashboard.Blazor.csproj`
- [?] (3) Review compilation errors (if any)
- [?] (4) Document all errors related to:
  - ConfigurationBinder.GetValue API changes
  - TimeSpan initialization issues
  - Any other API incompatibilities

**Validation**: Build attempted and all compilation errors identified

**Expected Errors**: 
- Binary incompatible: ConfigurationBinder.GetValue (5 locations)
- Source incompatible: TimeSpan.FromHours/FromMinutes (5 locations may cause errors)

**References**: Plan §4.1.4 Expected Breaking Changes

---

### [?] TASK-005: Fix API Compatibility Issues *(Completed: 2026-02-05 13:40)*
**Description**: Resolve all compilation errors from .NET 10 API breaking changes

**Actions**:
- [?] (1) Fix ConfigurationBinder.GetValue calls in Pages\Index.razor
  - Locate approximately 5 calls to `Configuration.GetValue<string>("key")`
  - Update to .NET 10 compatible pattern (e.g., use `Configuration["key"]` or new binding API)
  - Lines affected: ~311-314 in generated code
- [?] (2) Fix ConfigurationBinder.GetValue call in Modules\Tiles\Calendar.razor
  - Locate: `Configuration.GetValue<string>("GoogleCalendarEmbedCode")`
  - Update to .NET 10 compatible pattern
  - Line affected: ~83
- [?] (3) Verify TimeSpan initialization in timer code (fix only if compilation errors occur)
  - Files: SmartHome.razor, RandomJoke.razor, RandomQuote.razor, WeatherForecast.razor, SmartHomeTile.razor
  - Check: `TimeSpan.FromHours(1)` and `TimeSpan.FromMinutes(1)`
  - Add explicit type annotations if needed
- [?] (4) Build after each fix to verify error resolution
- [?] (5) Continue until all compilation errors resolved

**Validation**: Solution builds with 0 errors

**References**: Plan §4.1.5 Code Modifications Required

---

### [?] TASK-006: Final Build and Validation *(Completed: 2026-02-05 13:42)*
**Description**: Perform final build validation and prepare for testing

**Actions**:
- [?] (1) Clean build: `dotnet clean FamilyDashboard.sln`
- [?] (2) Restore packages: `dotnet restore FamilyDashboard.sln`
- [?] (3) Build solution: `dotnet build FamilyDashboard.sln`
- [?] (4) Verify build output:
  - 0 errors
  - 0 warnings (or only acceptable documented warnings)
  - Build succeeded message
- [?] (5) Check for vulnerable packages: `dotnet list package --vulnerable`
- [?] (6) Verify no security vulnerabilities reported

**Validation**: Clean successful build with no errors, no warnings, and no vulnerabilities

**References**: Plan §4.1.7 Validation Checklist (Build Phase)

---

### [?] TASK-007: Commit Changes *(Completed: 2026-02-05 13:43)*
**Description**: Commit all upgrade changes as a single atomic commit

**Actions**:
- [?] (1) Review all changed files: `git status`
- [?] (2) Stage all changes: `git add .`
- [?] (3) Commit with descriptive message:
  ```
  Upgrade FamilyDashboard to .NET 10
  
  - Update target framework from net7.0 to net10.0
  - Upgrade all NuGet packages to .NET 10 compatible versions:
    - Microsoft.AspNetCore.Components.WebAssembly: 7.0.5 ? 10.0.2
    - Microsoft.AspNetCore.Components.WebAssembly.DevServer: 7.0.5 ? 10.0.2
    - Microsoft.Extensions.Http: 7.0.0 ? 10.0.2
  - Fix ConfigurationBinder.GetValue API breaking changes
  - Build succeeds with 0 errors and 0 warnings
  ```
- [?] (4) Verify commit successful: `git log -1`

**Validation**: Changes committed successfully on upgrade-to-NET10 branch

**References**: Plan §8 Source Control Strategy

---

### [?] TASK-008: Manual Runtime Validation *(Completed: 2026-02-05 14:06)*
**Description**: Perform comprehensive manual testing of the upgraded application

**Actions**:
- [?] (1) Run the application locally
- [?] (2) Verify application starts without errors
- [?] (3) Test configuration-dependent features:
  - SmartThings tab visibility (with/without config)
  - Weather tab visibility (with/without config)
  - Calendar tab visibility (with/without config)
- [?] (4) Test SmartThings integration (if configured):
  - Device list loads
  - Device status displays
  - Commands execute successfully
- [?] (5) Test timer-based refreshes:
  - Weather tile (1-hour refresh)
  - SmartHome tile (1-hour refresh)
  - RandomQuote tile (1-hour refresh)
  - RandomJoke tile (1-hour refresh)
  - SmartHomeTile (1-minute refresh)
- [?] (6) Test data retrieval:
  - Weather API
  - Quote API
  - Joke API
  - Calendar embed
- [?] (7) Check browser console for errors
- [?] (8) Verify no runtime exceptions
- [?] (9) Confirm expected behavior for all behavioral changes

**Validation**: All features functional, no console errors, application behaves as expected

**References**: Plan §6 Testing & Validation Strategy

---

## Summary

**Progress**: 8/8 tasks completed (100%) ![100%](https://progress-bar.xyz/100)

**Next Task**: TASK-001 - Verify Prerequisites

**Notes**: 
- All tasks should be completed sequentially
- Do not proceed if any task fails validation
- Each task builds on the previous task's success
- The upgrade follows an All-At-Once strategy - all changes are interdependent

---

## Execution Log

_Task execution details will be tracked here as work progresses_
