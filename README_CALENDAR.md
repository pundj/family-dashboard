# ?? Family Dashboard - Google Calendar Integration (OAuth 2.0)

Your calendar has been upgraded from an embedded iframe to a full-featured, OAuth 2.0 authenticated calendar with Day, Week, and Month display options!

## ?? Quick Start (15 Minutes)

1. **Set Up OAuth 2.0** - See [QUICK_START.md](QUICK_START.md)
2. **Configure App** - Update `appsettings.Development.json`
3. **Sign In & Use** - Access your private calendars!

## ?? OAuth 2.0 Authentication

This implementation uses **OAuth 2.0** to securely access your **private** Google Calendars:

? No need to make calendars public
? Users sign in with their Google account  
? Secure token-based authentication
? Access persists across sessions
? Users can revoke access anytime

## ?? Documentation Guide

### Setup & Configuration
| Document | Purpose | When to Use |
|----------|---------|-------------|
| **[QUICK_START.md](QUICK_START.md)** | Fast 15-minute setup | Start here for OAuth setup |
| **[GOOGLE_CALENDAR_SETUP.md](GOOGLE_CALENDAR_SETUP.md)** | Detailed OAuth instructions | Complete step-by-step guide |
| **[OAUTH_TROUBLESHOOTING.md](OAUTH_TROUBLESHOOTING.md)** | Solve authentication issues | When you encounter errors |
| **[SETUP_CHECKLIST.md](SETUP_CHECKLIST.md)** | Step-by-step checklist | Ensure nothing is missed |
| **[CALENDAR_IDS_REFERENCE.md](CALENDAR_IDS_REFERENCE.md)** | Your existing calendar IDs | Copy/paste your calendar IDs |

### Usage & Technical
| Document | Purpose | When to Use |
|----------|---------|-------------|
| **[CALENDAR_USER_GUIDE.md](CALENDAR_USER_GUIDE.md)** | How to use the calendar | Learn about features |
| **[ARCHITECTURE.md](ARCHITECTURE.md)** | Technical architecture | For developers/customization |

## ? New Features

### ?? Three View Types
- **Day View** - Detailed list with times and locations
- **Week View** - 7-day grid calendar (Sunday or Monday start)
- **Month View** - Traditional monthly calendar

### ?? Easy Navigation
- Previous/Next buttons
- Jump to Today
- Switch views instantly
- Configurable week start day

### ?? Event Details
- Click any event to see full details
- View title, times, location, description
- Clean modal interface

### ?? Multi-Calendar Support
- Pull from multiple Google calendars
- All from same Google account
- Events merged and sorted automatically

## ?? Getting Started

### 1. Prerequisites
- Google account with calendars
- Access to [Google Cloud Console](https://console.cloud.google.com/)
- 15 minutes of time

### 2. OAuth 2.0 Setup

Follow the [QUICK_START.md](QUICK_START.md) guide to:
1. Create Google Cloud Project
2. Enable Calendar API
3. Configure OAuth consent screen
4. Create OAuth credentials (Client ID & Secret)
5. Update configuration

### 3. Configuration Example

```json
{
  "GoogleOAuth": {
    "ClientId": "123456-abc...xyz.apps.googleusercontent.com",
    "ClientSecret": "GOCSPX-...",
    "RedirectUri": "https://localhost:7104/oauth-callback",
    "CalendarIds": [
      "youremail@gmail.com",
      "anothercalendar@gmail.com"
    ]
  }
}
```

### 4. Run & Sign In

```bash
# Run the hosted API + Blazor frontend
dotnet run --project FamilyDashboard.Api/FamilyDashboard.Api.csproj

# In browser:
# 1. Open the dashboard
# 2. Go to the Calendar tab
# 3. Click "Sign in with Google"
# 4. Authorize access
# 5. View your private calendars!
```

## ?? Configuration Example

```json
{
  "FamilyName": "Smith",
  "Locale": "New York",
  "GoogleOAuth": {
    "ClientId": "123456789-abcdef.apps.googleusercontent.com",
    "ClientSecret": "GOCSPX-AbCdEf123456",
    "RedirectUri": "https://localhost:7104/oauth-callback",
    "CalendarIds": [
      "youremail@gmail.com",
      "calendar@group.calendar.google.com"
    ]
  }
}
```

## ?? What You Get

### Day View
```
???????????????????????????????????????
? 9:00 AM - 10:00 AM                 ?
? Team Meeting                        ?
? ?? Conference Room A                ?
???????????????????????????????????????
? 2:00 PM - 3:30 PM                  ?
? Project Review                      ?
? ?? Office Building                  ?
???????????????????????????????????????
```

### Week View
```
???????????????????????????????????????????
? Sun ? Mon ? Tue ? Wed ? Thu ? Fri ? Sat ?
?  15 ?  16 ?  17 ?  18 ?  19 ?  20 ?  21 ?
???????????????????????????????????????????
?     ?9:00 ?     ?     ?     ?     ?     ?
?     ?Meet ?     ?     ?     ?     ?     ?
???????????????????????????????????????????
```

### Month View
```
????????????????????????????????????
?Sun ?Mon ?Tue ?Wed ?Thu ?Fri ?Sat ?
????????????????????????????????????
?  1 ?  2 ?  3 ?  4 ?  5 ?  6 ?  7 ?
?    ?Meet?    ?    ?    ?    ?    ?
????????????????????????????????????
?  8 ?  9 ? 10 ? 11 ? 12 ? 13 ? 14 ?
?    ?    ?    ?    ?    ?Party?    ?
????????????????????????????????????
```

## ??? Technical Stack

- **Frontend**: Blazor WebAssembly (.NET 10)
- **API**: Google Calendar API v3
- **Authentication**: OAuth 2.0 (Authorization Code Flow)
- **Token Storage**: Browser Local Storage
- **Architecture**: Service-based with DI
- **NuGet Packages**:
  - `Google.Apis.Calendar.v3`
  - `Google.Apis.Auth`

### Project Structure
```
FamilyDashboard.Blazor/
??? Models/Calendar/
?   ??? CalendarEvent.cs
?   ??? CalendarViewType.cs
?   ??? WeekStartDay.cs
?   ??? GoogleOAuthSettings.cs
?   ??? OAuthToken.cs
??? Services/
?   ??? ICalendarService.cs
?   ??? GoogleCalendarService.cs
?   ??? IGoogleAuthService.cs
?   ??? GoogleAuthService.cs
??? Modules/Tiles/
?   ??? Calendar.razor (main component)
?   ??? DayView.razor
?   ??? WeekView.razor
?   ??? MonthView.razor
??? Pages/
?   ??? OAuthCallback.razor
??? wwwroot/
    ??? appsettings.json (configuration)
```

## ?? Security Considerations

### OAuth 2.0 Authentication
? **Secure**: Industry-standard authentication
? **Private**: Access private calendars without making them public
? **User-controlled**: Users can revoke access anytime
? **Token-based**: Temporary access tokens, automatically refreshed

### Token Storage
**Where**: Browser localStorage
**Security**: 
- Tokens only accessible to your domain
- HTTPS required (localhost exempt for dev)
- Access tokens expire after 1 hour
- Refresh tokens persist until revoked

### Best Practices
1. **Never commit secrets to Git**
   - Add `appsettings.Development.json` to `.gitignore`
   - Use environment variables in production
   - Consider Azure Key Vault for production

2. **Restrict OAuth Client**
   - Only add necessary redirect URIs
   - Limit to actual domains
   - Monitor usage in Google Cloud Console

3. **Production Security**
   - Use HTTPS (required)
   - Implement Content Security Policy
   - Consider server-side token storage
   - Add encryption for sensitive data

4. **User Privacy**
   - Users can revoke access: [Google Account Permissions](https://myaccount.google.com/permissions)
   - Clear explanation of permissions during sign-in
   - Sign-out functionality provided

## ?? Troubleshooting

### Sign-In Issues
**"Redirect URI mismatch"**
- Check redirect URI matches Google Cloud Console exactly
- Should be: `https://localhost:7104/oauth-callback`
- Verify port number and path

**"App isn't verified" warning**
- Normal for development - click "Advanced" ? "Go to app (unsafe)"
- Optional: Submit for verification in production

**Sign-in button does nothing**
- Check browser console (F12) for errors
- Verify ClientId in configuration
- Try hard refresh (Ctrl+Shift+R)

### No Events Showing
1. Check browser console (F12) for errors
2. Verify Calendar IDs in configuration are correct
3. Ensure calendars exist and have events
4. Try different view (Day/Week/Month)
5. Sign out and sign in again

### Authentication Persists  Issues
**Can't stay signed in**
- Check browser allows localStorage
- Disable privacy extensions temporarily
- Check cookies are enabled

**Tokens expired**
- App should auto-refresh - if not, sign in again
- Check browser console for refresh errors

**See [OAUTH_TROUBLESHOOTING.md](OAUTH_TROUBLESHOOTING.md) for comprehensive troubleshooting guide.**

## ?? Features Comparison

| Feature | Old (Embedded) | New (API-Driven) |
|---------|---------------|------------------|
| View Options | 1 (Agenda) | 3 (Day/Week/Month) |
| Calendars | Multiple | Multiple |
| Customization | Limited | Full control |
| Responsive | Limited | Fully responsive |
| Event Details | In Google | Modal popup |
| Navigation | Limited | Full control |
| Styling | Google's | Custom |

## ?? Future Enhancements

Potential features to add:
- [ ] Event color coding by calendar
- [ ] Filter/toggle calendars
- [ ] Event search
- [ ] Create/edit events
- [ ] OAuth 2.0 for private calendars
- [ ] Offline support
- [ ] Export to ICS
- [ ] Keyboard shortcuts
- [ ] Print view
- [ ] Recurring event details

## ?? Support & Help

### Documentation
- All guides are in the root directory
- Start with [QUICK_START.md](QUICK_START.md)
- Use [SETUP_CHECKLIST.md](SETUP_CHECKLIST.md) to track progress

### External Resources
- [Google Calendar API Docs](https://developers.google.com/calendar/api/v3/reference)
- [Google Cloud Console](https://console.cloud.google.com/)

### Common Issues
See the troubleshooting section in [GOOGLE_CALENDAR_SETUP.md](GOOGLE_CALENDAR_SETUP.md)

## ?? What Changed

From your old configuration, we:
- ? Replaced embedded iframe with API integration
- ? Added Day, Week, and Month views
- ? Created interactive event details
- ? Added navigation controls
- ? Built responsive, modern UI
- ? Maintained multi-calendar support

**Your calendar IDs** from the old embed code have been documented in [CALENDAR_IDS_REFERENCE.md](CALENDAR_IDS_REFERENCE.md) for easy migration.

## ?? Ready to Start?

1. **Fast Track**: Follow [QUICK_START.md](QUICK_START.md) - 5 minutes
2. **Thorough**: Use [SETUP_CHECKLIST.md](SETUP_CHECKLIST.md) - 15 minutes
3. **Learn More**: Read [GOOGLE_CALENDAR_SETUP.md](GOOGLE_CALENDAR_SETUP.md)

**Happy scheduling! ??**
