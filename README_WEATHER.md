# Family Dashboard Weather

This document explains how to configure and use the Weather section of Family Dashboard.

## How It Works

- Weather data is fetched from Open-Meteo using your configured latitude/longitude.
- Current conditions, hourly forecast, and day-to-day forecast are displayed in the Weather tile.
- Weather alerts are pulled from the National Weather Service when available.
- Overnight clear conditions are shown as clear/moon instead of sunny/sun.

## Configuration

Set these values in `FamilyDashboard.Blazor/wwwroot/appsettings.json` or `appsettings.Development.json`:

```json
{
  "Weather": {
    "Latitude": 40.707188,
    "Longitude": -74.010723,
    "LocationName": "New York, NY"
  }
}
```

### Settings

- `Weather:Latitude` - required for forecast lookup
- `Weather:Longitude` - required for forecast lookup
- `Weather:LocationName` - optional label shown in the UI

## What You Get

- Current temperature and condition
- Humidity, wind, precipitation, sunrise, and sunset
- Hourly forecast with paging
- Day-to-day forecast with paging
- Weather alerts in a scrollable popup

## Troubleshooting

### Weather data does not load

1. Confirm `Weather:Latitude` and `Weather:Longitude` are set.
2. Confirm the app can reach Open-Meteo and NWS.
3. Restart the app after changing config.

### Alerts are missing

1. Alerts are only shown when the NWS endpoint returns active alerts for your location.
2. If NWS is unavailable, the app shows alerts as temporarily unavailable.

### Overnight icons look wrong

Clear nighttime weather is mapped to a moon icon automatically when the forecast time is outside sunrise/sunset.
