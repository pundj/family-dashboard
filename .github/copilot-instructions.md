# Copilot Instructions

## Project Guidelines
- This repository is a hosted **Blazor WebAssembly** dashboard app targeting **.NET 10**. Prefer Blazor component patterns over MVC/Razor Pages patterns.
- Keep changes minimal and focused on the requested feature or bug.
- Preserve the existing dashboard tab UX and styling conventions.

## Cameras Tab Behavior
- For the Family Dashboard Cameras tab UX, prefer clicking the Cameras tab to immediately open cameras in a popup/new window rather than navigating to an intermediate dashboard page with a button.
- Do not attempt to embed providers that block framing (for example CSP `frame-ancestors 'none'`). Use popup/new window behavior instead.

## Configuration Expectations
- Client configuration is loaded from `FamilyDashboard.Blazor/wwwroot/appsettings*.json`.
- `CameraViewerUrl` must be a **top-level** config key (not nested under `GoogleOAuth`) and must be an absolute `http` or `https` URL.
- Never hardcode secrets or tokens in code changes.

## Safety and Source Control Rules
- Never commit directly to `main`.
- Never create commits without explicit user permission.
- Never push to any remote without explicit user permission.
- If a commit or push is requested, confirm scope and target branch first.
