# OAuth 2.0 Quick Reference Card

## ?? Important Links

| Resource | URL |
|----------|-----|
| Google Cloud Console | https://console.cloud.google.com/ |
| OAuth Consent Screen | Console ? APIs & Services ? OAuth consent screen |
| Credentials | Console ? APIs & Services ? Credentials |
| Google Calendar | https://calendar.google.com/ |
| Revoke Access | https://myaccount.google.com/permissions |

## ?? Configuration Template

```json
{
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

## ?? OAuth Setup Checklist

- [ ] Create Google Cloud Project
- [ ] Enable Google Calendar API
- [ ] Configure OAuth consent screen
  - [ ] App name
  - [ ] Support email
  - [ ] Developer email
  - [ ] Add scope: `calendar.readonly`
- [ ] Create OAuth client
  - [ ] Type: Web application
  - [ ] Add redirect URI
- [ ] Copy Client ID and Secret
- [ ] Update `appsettings.Development.json`
- [ ] Add Calendar IDs
- [ ] Test sign-in flow

## ?? Required OAuth Scope

```
https://www.googleapis.com/auth/calendar.readonly
```

**Grants**: Read-only access to calendars

## ?? Redirect URIs

**Development**: `https://localhost:7104/oauth-callback`  
**Production**: `https://yourdomain.com/oauth-callback`

?? Must match EXACTLY (case-sensitive, no trailing slash)

## ?? OAuth Flow Steps

1. User clicks "Sign in with Google"
2. Redirect to Google authorization URL
3. User authenticates
4. User grants permission
5. Google redirects with authorization code
6. App exchanges code for tokens
7. Tokens stored in browser localStorage
8. Access token used for API requests
9. Refresh token renews expired access token

## ?? Token Lifecycle

| Token Type | Lifespan | Purpose |
|-----------|----------|---------|
| Access Token | 1 hour | API requests |
| Refresh Token | Until revoked | Get new access tokens |

**Auto-refresh**: Happens 5 minutes before expiry

## ??? Debugging Commands

### Check Token in Browser Console
```javascript
// View token
localStorage.getItem('google_oauth_token')

// View parsed token
JSON.parse(localStorage.getItem('google_oauth_token'))

// Clear storage
localStorage.clear()
```

### Check Network Requests (F12 ? Network)
- Filter by: `calendar` or `oauth`
- Look for: Status codes, headers, responses
- Auth header should be: `Authorization: Bearer ya29...`

## ?? Common Errors & Quick Fixes

| Error | Quick Fix |
|-------|-----------|
| `redirect_uri_mismatch` | Check exact match in Google Console |
| `invalid_client` | Verify Client ID & Secret |
| `access_denied` | User clicked deny - try again |
| App isn't verified | Click "Advanced" ? "Go to app" (normal) |
| No events after sign-in | Check Calendar IDs, browser console |
| Can't sign in | Check Client ID in config |
| Can't stay signed in | Check localStorage permissions |

## ?? Key Files

| File | Purpose |
|------|---------|
| `appsettings.Development.json` | OAuth configuration |
| `GoogleAuthService.cs` | OAuth flow logic |
| `GoogleCalendarService.cs` | API requests |
| `Calendar.razor` | Sign-in UI |
| `OAuthCallback.razor` | Handle redirect |

## ?? Security Checklist

- [ ] Never commit `appsettings.Development.json` to Git
- [ ] Add to `.gitignore`
- [ ] Use environment variables in production
- [ ] Restrict redirect URIs to actual domains
- [ ] Monitor usage in Google Cloud Console
- [ ] Use HTTPS in production (required)
- [ ] Educate users about permission revocation

## ?? Get Help

| Issue Type | Resource |
|------------|----------|
| Setup | `QUICK_START.md` |
| Detailed steps | `GOOGLE_CALENDAR_SETUP.md` |
| Errors | `OAUTH_TROUBLESHOOTING.md` |
| What changed | `OAUTH_IMPLEMENTATION_SUMMARY.md` |
| Using calendar | `CALENDAR_USER_GUIDE.md` |

## ?? Time Estimates

| Task | Time |
|------|------|
| Google Cloud setup | 10 min |
| Configuration | 2 min |
| Testing | 3 min |
| **Total** | **15 min** |

## ?? User Experience

### First Time
1. See "Sign in with Google" button
2. Click ? redirect to Google
3. Sign in with Google account
4. Grant calendar permission
5. Redirect back ? see events

### Returning Users
1. Open app
2. Already signed in
3. Events load automatically
4. Can sign out if desired

## ?? OAuth vs API Key

| Feature | API Key | OAuth 2.0 |
|---------|---------|-----------|
| Private calendars | ? | ? |
| User sign-in | ? | ? |
| Setup time | 5 min | 15 min |
| Security | Basic | High |
| User control | ? | ? |

## ?? Production Deployment

1. Add production redirect URI to OAuth client
2. Update `RedirectUri` in production config
3. Store secrets in environment variables
4. Use HTTPS (required)
5. Optional: Submit for app verification

## ?? Emergency Fixes

### Everything Broken?
1. Clear browser localStorage
2. Sign out
3. Verify configuration
4. Sign in again

### Can't Sign In?
1. Check Client ID is correct
2. Check redirect URI matches exactly
3. Clear browser cache
4. Try incognito mode

### No Events?
1. Check browser console (F12)
2. Verify Calendar IDs
3. Sign out and back in
4. Check Google Cloud Console for errors

---

**Keep this handy during setup!** ??
