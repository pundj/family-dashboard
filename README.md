# Family Dashboard

A hosted Blazor WebAssembly application for displaying family information, smart home controls, weather, calendar, and more.

## Technology Stack

- **.NET 10.0** (Long Term Support)
- **Blazor WebAssembly** - Client-side web UI framework
- **ASP.NET Core 10.0** - Web hosting and API integration

## Prerequisites

### Required

- **.NET 10 SDK** (version 10.0.102 or later)
  - Download: [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
  - Verify installation: `dotnet --version`
- **Node.js LTS** (includes npm)
  - Required to bundle the Blazor client's JavaScript and Sass assets.
  - Download: [Node.js](https://nodejs.org/)
  - Verify installation: `node --version` and `npm --version`

### Optional

- **Visual Studio 2026** or later
- **Visual Studio Code** with C# extension

## Getting Started

### Clone the Repository

```bash
git clone https://github.com/pundj/family-dashboard.git
cd family-dashboard
```

### Build the Application

```bash
# Restore frontend dependencies
cd FamilyDashboard.Blazor
npm ci
cd ..

# Restore and build .NET dependencies
dotnet restore
dotnet build
```

### Run the Application

```bash
# Run the hosted API + Blazor frontend
dotnet run --project FamilyDashboard.Api/FamilyDashboard.Api.csproj
```

The application will be available at:
- **HTTPS**: https://localhost:7104
- **HTTP**: http://localhost:5298

### Run with .NET Aspire (recommended for local development)

```bash
dotnet run --project FamilyDashboard.AppHost/FamilyDashboard.AppHost.csproj
```

Aspire AppHost orchestrates the API host and provides the local Aspire dashboard for service health and logs.

### Run with Docker

```bash
docker compose up --build
```

The app will be available at http://localhost:7104 and persists API data in `FamilyDashboard.Api/App_Data`.

## Features

- **Dashboard Interface** - Tabbed interface for different information types
- **Smart Home Integration** - Per-user SmartThings token storage with server-side proxying
- **Weather Forecast** - Native weather data display using Open-Meteo and NWS alerts (configurable)
- **Calendar** - Google Calendar OAuth integration with Day/Week/Month views (configurable)
- **Cameras** - Optional Cameras tab that launches a configured camera viewer URL in a popup window
- **Random Quote/Joke** - Entertainment tiles with auto-refresh
- **Auto-refresh** - Tiles automatically update at configured intervals
- **Inactivity Screen Timeout** - A configurable screen-darkening timeout that wakes when the screen is touched

## Configuration

The application uses configuration files for non-secret settings and server-side storage for per-user SmartThings tokens:

- **SmartThings**: Configure users in-app (Smart Home tab) and store SmartThings tokens server-side in the API database (encrypted via ASP.NET Data Protection).
- **Weather**: Configure `Weather:Latitude`, `Weather:Longitude`, and optional `Weather:LocationName` in `FamilyDashboard.Blazor/wwwroot/appsettings.json`
- **Weather**: See [README_WEATHER.md](README_WEATHER.md) for setup and usage details.
- **Calendar**: Configure `GoogleOAuth` for private calendar access (or use `GoogleCalendarEmbedCode` for legacy embedded calendar mode).
- **Cameras**: Set `CameraViewerUrl` as a **top-level** key in `FamilyDashboard.Blazor/wwwroot/appsettings*.json` using an absolute `http`/`https` URL. The Cameras tab opens this URL in a popup window.
- **Screen timeout and screensaver display**: Select the gear button in the dashboard header to configure **Screen timeout (minutes)** plus the screensaver's date/time display options. The timeout defaults to five minutes, accepts values from 1 to 1,440 minutes, and all dashboard display preferences are stored in that browser's local storage. When the timeout expires, touch or click the dark screen to resume the dashboard.

For camera-specific setup and troubleshooting, see [README_CAMERAS.md](README_CAMERAS.md).
For weather-specific setup and troubleshooting, see [README_WEATHER.md](README_WEATHER.md).

## Project Structure

```
family-dashboard/
??? FamilyDashboard.Api/             # ASP.NET Core host + API + auth + SmartThings proxy
??? FamilyDashboard.Blazor/          # Blazor WebAssembly client project
??? FamilyDashboard.AppHost/         # Aspire orchestration host for local development
??? FamilyDashboard.ServiceDefaults/ # Shared Aspire defaults (telemetry, health checks, resilience)
?   ??? Pages/                        # Razor pages/components
?   ??? Modules/                      # Feature modules
?   ?   ??? Tiles/                    # Dashboard tile components
?   ??? Services/                     # API integration services
?   ??? wwwroot/                      # Static web assets
??? .github/                          # GitHub metadata
?   ??? upgrades/                     # Upgrade documentation
??? README.md                         # This file
```

## Recent Updates

### .NET 10 Upgrade (February 2026)

The application has been successfully upgraded to .NET 10.0 LTS:

- **Framework**: Upgraded from .NET 7.0 to .NET 10.0
- **Packages**: Updated all ASP.NET Core packages to version 10.0.2
  - `Microsoft.AspNetCore.Components.WebAssembly` ? 10.0.2
  - `Microsoft.AspNetCore.Components.WebAssembly.DevServer` ? 10.0.2
  - `Microsoft.Extensions.Http` ? 10.0.2
- **Compatibility**: All APIs verified compatible with .NET 10
- **Security**: No vulnerable dependencies

For detailed upgrade information, see [upgrade documentation](.github/upgrades/).

## Development

### Build for Production

```bash
dotnet publish FamilyDashboard.Api/FamilyDashboard.Api.csproj -c Release
```

Output will be in `FamilyDashboard.Api/bin/Release/net10.0/publish/`.

### Run Tests

```bash
# If tests are available
dotnet test
```


## SmartThings Security Architecture

- Users register/sign in using the backend auth endpoints (`/api/auth/*`).
- Each signed-in user can save/replace/remove their own SmartThings personal access token from the Smart Home tab.
- Tokens are stored server-side in SQLite (`FamilyDashboard.Api/App_Data/familydashboard.db`) and encrypted using ASP.NET Core Data Protection before persistence.
- The browser never reads stored SmartThings tokens after submission.
- SmartThings device reads and commands are proxied through authenticated backend endpoints (`/api/me/smartthings/*`).

## Azure App Service Deployment Notes

This repository now supports a single deployable app (`FamilyDashboard.Api`) suitable for Azure App Service:

1. Deploy `FamilyDashboard.Api` to App Service.
2. Persist app data and data-protection keys using durable storage in production (for example, Azure SQL/Azure Blob + Key Vault) instead of local file storage.
3. Keep SmartThings tokens server-side only; no token values are required in `FamilyDashboard.Blazor/wwwroot/appsettings.json`.
4. Ensure HTTPS is enabled (required for secure auth cookies).

For container-based Azure hosting, build from `FamilyDashboard.Api/Dockerfile` and deploy the image to Azure Container Apps or Azure App Service for Containers.

## Browser Support

Blazor WebAssembly requires a modern browser with WebAssembly support:

- Chrome/Edge (latest)
- Firefox (latest)
- Safari (latest)

## License

[Specify your license here]

## Contributing

[Add contribution guidelines if applicable]

## Support

For issues or questions, please open an issue on GitHub.