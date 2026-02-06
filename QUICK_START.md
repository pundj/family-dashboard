# Google Calendar OAuth 2.0 - Quick Setup (15 Minutes)

This guide helps you set up OAuth 2.0 authentication to access your **private** Google Calendars.

## Prerequisites
- A Google account with calendars you want to display
- Access to [Google Cloud Console](https://console.cloud.google.com/)
- 15 minutes of time

## Setup Steps

### 1. Create Google Cloud Project (2 minutes)
1. Go to https://console.cloud.google.com/
2. Click the project dropdown → "New Project"
3. Name: "Family Dashboard" → Click "Create"
4. Make sure your new project is selected

### 2. Enable Calendar API (1 minute)
1. Left menu: "APIs & Services" → "Library"
2. Search: "Google Calendar API"
3. Click it → Click "Enable"

### 3. Configure OAuth Consent Screen (5 minutes)

#### Basic Info
1. Left menu: "APIs & Services" → "OAuth consent screen"
2. Select "External" → Click "Create"
3. Fill in:
   - App name: **Family Dashboard**
   - User support email: **Your email**
   - Developer contact: **Your email**
4. Click "Save and Continue"

#### Scopes
1. Click "Add or Remove Scopes"
2. Search for "calendar"
3. Check ✓ `.../auth/calendar.readonly`
4. Click "Update" → "Save and Continue"

#### Test Users (Optional)
1. Click "+ Add Users"
2. Enter your email
3. Click "Add" → "Save and Continue"
4. Click "Back to Dashboard"

### 4. Create OAuth Credentials (3 minutes)
1. Left menu: "APIs & Services" → "Credentials"
2. Click "+ CREATE CREDENTIALS" → "OAuth client ID"
3. Application type: **Web application**
4. Name: **Family Dashboard Web Client**
5. Under "Authorized redirect URIs":
   - Click "+ Add URI"
   - Enter: `https://localhost:7104/oauth-callback`
   - ⚠️ Must be exact (including `/oauth-callback`)
6. Click "Create"
7. **Copy your credentials**:
   - Client ID: `123456...apps.googleusercontent.com`
   - Client secret: `GOCSPX-...`
8. Click "OK"

### 5. Get Calendar IDs (2 minutes per calendar)
1. Go to https://calendar.google.com/
2. For each calendar you want to show:
   - Click ⋮ (three dots) next to calendar name
   - Click "Settings and sharing"
   - Scroll to "Integrate calendar"
   - Copy the "Calendar ID"
   - Your main calendar ID is usually your email

### 6. Configure Your App (2 minutes)
1. Open: `FamilyDashboard.Blazor\wwwroot\appsettings.Development.json`
2. Update this section:

```json
{
  "GoogleOAuth": {
    "ClientId": "PASTE_YOUR_CLIENT_ID_HERE",
    "ClientSecret": "PASTE_YOUR_CLIENT_SECRET_HERE",
    "RedirectUri": "https://localhost:7104/oauth-callback",
    "CalendarIds": [
      "youremail@gmail.com",
      "anothercalendar@gmail.com"
    ]
  }
}
```

### 7. Test (2 minutes)
1. Run your app: `dotnet run --project FamilyDashboard.Blazor`
2. Go to Calendar tile
3. Click "Sign in with Google"
4. Sign in with your Google account
5. Click "Advanced" → "Go to Family Dashboard (unsafe)" (this is normal for dev)
6. Click "Allow" to grant calendar access
7. You should see your events!

## Example Configuration

```json
{
  "FamilyName": "Smith",
  "Locale": "New York",
  "GoogleOAuth": {
    "ClientId": "790478070066-abc123xyz.apps.googleusercontent.com",
    "ClientSecret": "GOCSPX-ABC123XYZ",
    "RedirectUri": "https://localhost:7104/oauth-callback",
    "CalendarIds": [
      "john.smith@gmail.com",
      "jane.smith@gmail.com",
      "en.usa#holiday@group.v.calendar.google.com"
    ]
  }
}
```

## Troubleshooting

❌ **"Redirect URI mismatch" error?**
→ Check redirect URI in Google Cloud Console exactly matches: `https://localhost:7104/oauth-callback`
→ Check your app's port number (might not be 7104)
→ No trailing slash!

❌ **"App isn't verified" warning?**
→ This is normal for development
→ Click "Advanced" → "Go to Family Dashboard (unsafe)"
→ For production, optionally submit for verification

❌ **Sign in button does nothing?**
→ Check browser console (F12) for errors
→ Verify ClientId is correct in config
→ Verify RedirectUri matches Google Cloud Console

❌ **No events after sign in?**
→ Check browser console for errors
→ Verify Calendar IDs are correct
→ Make sure calendars have events in the selected date range

❌ **Sign out doesn't work?**
→ Click "Sign Out" button at top of calendar
→ Should return to "Sign in with Google" screen
→ Check browser console for errors

## Important Notes

✅ **Private Calendars**: No need to make calendars public!
✅ **Secure**: Users authenticate with their own Google account
✅ **Persistent**: Sign-in persists across page refreshes
✅ **Multiple Users**: Each user signs in with their own account

⚠️ **Security**:
- Never commit Client Secret to Git
- Tokens stored in browser local storage
- Users can revoke access anytime at [Google Account](https://myaccount.google.com/permissions)

## Production Deployment

When deploying to production:

1. **Add Production Redirect URI**:
   - Go to Google Cloud Console → Credentials
   - Edit your OAuth client
   - Add: `https://yourdomain.com/oauth-callback`

2. **Update Configuration**:
   - Create `appsettings.Production.json`
   - Update RedirectUri to production URL
   - Store secrets in environment variables, not config files

## What Happens During Sign In?

1. Click "Sign in with Google"
2. Redirected to Google's sign-in page
3. Enter Google credentials
4. Google shows permissions (read calendars)
5. Click "Allow"
6. Redirected back to your app
7. App exchanges code for access tokens
8. Tokens stored in browser
9. Calendar loads your events!

## Understanding OAuth 2.0 vs API Key

| Feature | API Key | OAuth 2.0 |
|---------|---------|-----------|
| Calendar Access | Public only | Public & Private |
| Authentication | None | User signs in |
| Security | Basic | High |
| Setup | Easier | More steps |
| Best For | Public calendars | Private calendars |

## Need More Help?

- **Detailed Guide**: See `GOOGLE_CALENDAR_SETUP.md`
- **Step-by-Step**: See `SETUP_CHECKLIST.md`
- **Calendar IDs**: See `CALENDAR_IDS_REFERENCE.md`
- **How to Use**: See `CALENDAR_USER_GUIDE.md`

## Quick Reference

**OAuth Consent Screen**: Google Cloud Console → APIs & Services → OAuth consent screen
**Credentials**: Google Cloud Console → APIs & Services → Credentials
**Config File**: `FamilyDashboard.Blazor\wwwroot\appsettings.Development.json`
**Redirect URI**: `https://localhost:7104/oauth-callback`
**Calendar IDs**: Google Calendar → Settings → Integrate calendar

---

**Ready to start? Follow the steps above, and you'll be viewing your private calendars in 15 minutes!** 🎉
