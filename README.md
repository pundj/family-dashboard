# Family Dashboard

A Blazor WebAssembly application for displaying family information, smart home controls, weather, calendar, and more.

## Technology Stack

- **.NET 10.0** (Long Term Support)
- **Blazor WebAssembly** - Client-side web UI framework
- **ASP.NET Core 10.0** - Web hosting and API integration

## Prerequisites

### Required

- **.NET 10 SDK** (version 10.0.102 or later)
  - Download: [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
  - Verify installation: `dotnet --version`

### Optional (for development)

- **Visual Studio 2026** or later
- **Visual Studio Code** with C# extension
- **Node.js** (for frontend build tasks)

## Getting Started

### Clone the Repository

```bash
git clone https://github.com/pundj/family-dashboard.git
cd family-dashboard
```

### Build the Application

```bash
# Restore dependencies
dotnet restore

# Build the solution
dotnet build
```

### Run the Application

```bash
# Run the Blazor WebAssembly application
dotnet run --project FamilyDashboard.Blazor/FamilyDashboard.Blazor.csproj
```

The application will be available at:
- **HTTPS**: https://localhost:7104
- **HTTP**: http://localhost:5298

## Features

- **Dashboard Interface** - Tabbed interface for different information types
- **Smart Home Integration** - SmartThings device control (configurable)
- **Weather Forecast** - Local weather display (configurable)
- **Calendar** - Google Calendar embed (configurable)
- **Random Quote/Joke** - Entertainment tiles with auto-refresh
- **Auto-refresh** - Tiles automatically update at configured intervals

## Configuration

The application uses `appsettings.json` for configuration. Features are enabled based on configuration presence:

- **SmartThings**: Requires `SmartThingsApiBaseAddress` and `SmartThingsApiAccessToken`
- **Weather**: Requires `Locale` configuration
- **Calendar**: Requires `GoogleCalendarEmbedCode`

## Project Structure

```
family-dashboard/
??? FamilyDashboard.Blazor/          # Blazor WebAssembly project
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
dotnet publish FamilyDashboard.Blazor/FamilyDashboard.Blazor.csproj -c Release
```

Output will be in `FamilyDashboard.Blazor/bin/Release/net10.0/publish/`

### Run Tests

```bash
# If tests are available
dotnet test
```

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