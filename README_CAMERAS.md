# Family Dashboard Cameras Tab

This document explains how to configure and use the optional Cameras tab.

## How It Works

- The Cameras tab is shown only when `CameraViewerUrl` is configured.
- Clicking the Cameras tab opens the configured URL in a popup/new browser window.
- The dashboard does **not** embed camera pages in an iframe.

## Why Popup Instead of Embed

Many camera providers (including Wyze Web View) block iframe embedding with Content Security Policy headers such as:

- `frame-ancestors 'none'`

When that policy is present, browsers block embedding and there is no client-side workaround in Blazor.

## Configuration

Set `CameraViewerUrl` as a **top-level** key in your Blazor client config file (or an environment override file you create):

- `FamilyDashboard.Blazor/wwwroot/appsettings.json`
- `FamilyDashboard.Blazor/wwwroot/appsettings.{Environment}.json` (optional; create for environment-specific overrides)

Example:

```json
{
  "FamilyName": "Smith",
  "Locale": "Jefferson City",
  "CameraViewerUrl": "https://my.wyze.com/home",
  "GoogleOAuth": {
	"ClientId": null,
	"ClientSecret": null,
	"RedirectUri": null,
	"CalendarIds": [],
	"CalendarNames": {}
  }
}
```

## Requirements

- The URL must be absolute (`http://` or `https://`).
- Browser popup permissions must allow windows from your dashboard origin.

## Troubleshooting

### Cameras tab is missing

1. Confirm `CameraViewerUrl` exists at the root level (not under `GoogleOAuth`).
2. Confirm the URL is absolute and valid.
3. Restart the app after config changes.

### Popup does not appear

1. Allow popups for the dashboard site in browser settings.
2. Check if popup blockers/extensions are enabled.

### Iframe/CSP error appears in console

This is expected when the provider forbids embedding. Use popup behavior.
