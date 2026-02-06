# OAuth 2.0 Troubleshooting Guide

Comprehensive troubleshooting for Google Calendar OAuth 2.0 authentication.

## Table of Contents
1. [Common Error Messages](#common-error-messages)
2. [Setup Issues](#setup-issues)
3. [Sign-In Problems](#sign-in-problems)
4. [Token & API Issues](#token--api-issues)
5. [Browser & Network Issues](#browser--network-issues)
6. [Configuration Problems](#configuration-problems)

---

## Common Error Messages

### "redirect_uri_mismatch"

**Full Error**: `Error 400: redirect_uri_mismatch`

**Cause**: The redirect URI in your request doesn't match what's configured in Google Cloud Console.

**Solutions**:

1. **Check Google Cloud Console**:
   - Go to APIs & Services ? Credentials
   - Click on your OAuth client
   - Look at "Authorized redirect URIs"
   - Should include: `https://localhost:7104/oauth-callback`

2. **Check your configuration**:
   ```json
   "RedirectUri": "https://localhost:7104/oauth-callback"
   ```
   - Must match EXACTLY (case-sensitive)
   - Check port number (might not be 7104 for you)
   - Include `/oauth-callback` at the end
   - Use `https`, not `http`
   - No trailing slash

3. **Find your actual port**:
   - Run your app
   - Look at browser address bar: `https://localhost:XXXX`
   - Use that port number
   - Add `/oauth-callback`

4. **Common mistakes**:
   - ? `http://localhost:7104/oauth-callback` (http instead of https)
   - ? `https://localhost:7104/oauth-callback/` (trailing slash)
   - ? `https://localhost:7104` (missing /oauth-callback)
   - ? `https://localhost:7104/oauth-callback` (correct)

### "access_denied"

**Error**: User sees "Access denied" or similar

**Causes & Solutions**:

1. **User clicked "Cancel" or "Deny"**
   - Solution: Try signing in again
   - Click "Allow" when prompted

2. **App not authorized for user**
   - Check OAuth consent screen test users
   - Add user's email to test users list

3. **Incorrect scopes**
   - Verify scope in OAuth consent screen
   - Should include: `https://www.googleapis.com/auth/calendar.readonly`

### "invalid_client"

**Error**: `Error 401: invalid_client`

**Causes & Solutions**:

1. **Wrong Client ID or Secret**
   - Double-check in configuration file
   - Copy from Google Cloud Console again
   - No extra spaces or line breaks

2. **Wrong OAuth client type**
   - Must be "Web application", not "Desktop" or "Mobile"
   - Recreate if wrong type

3. **Client deleted or disabled**
   - Check if client exists in Google Cloud Console
   - Create new one if needed

### "invalid_grant"

**Error**: `Error 400: invalid_grant`

**Causes**:
- Authorization code already used
- Authorization code expired
- System clock out of sync

**Solutions**:
1. Sign out and sign in again
2. Check system time is correct
3. Clear browser storage
4. Refresh the page

### "origin_mismatch"

**Error**: Origin mismatch

**Causes**: JavaScript origin doesn't match authorized origins

**Solutions**:
1. Usually not needed for server-side OAuth
2. If using JavaScript SDK, add authorized JavaScript origins
3. For localhost: `https://localhost:7104`

---

## Setup Issues

### Can't Find OAuth Consent Screen

**Problem**: Can't find OAuth consent screen in Google Cloud Console

**Solutions**:
1. Make sure correct project is selected (dropdown at top)
2. Navigate: ? ? "APIs & Services" ? "OAuth consent screen"
3. If not showing, enable Calendar API first
4. Refresh the page

### "App isn't verified" Warning

**Warning**: Google shows "This app isn't verified" screen

**This is NORMAL for development!**

**What to do**:
1. Click "Advanced" (bottom left)
2. Click "Go to [App Name] (unsafe)"
3. Click "Allow"

**For production**:
- Optional: Submit for verification
- Required only for apps with many users
- Personal/family apps don't need verification

### Can't Add Redirect URI

**Problem**: Can't save redirect URI in OAuth client

**Solutions**:
1. Check format: `https://localhost:7104/oauth-callback`
2. Must use https (localhost exception exists)
3. No wildcards allowed
4. No query parameters allowed
5. Path is case-sensitive

### Calendar API Not Showing in Restrictions

**Problem**: Can't find Google Calendar API when restricting key

**Solutions**:
1. Make sure Calendar API is enabled first
2. Wait a few minutes after enabling
3. Refresh the credentials page
4. Try in incognito mode

---

## Sign-In Problems

### Sign In Button Does Nothing

**Problem**: Clicking "Sign in with Google" has no effect

**Check**:
1. Browser console (F12) for JavaScript errors
2. Network tab for failed requests
3. ClientId in configuration is correct

**Solutions**:
1. Hard refresh: Ctrl+Shift+R (Windows) or Cmd+Shift+R (Mac)
2. Clear browser cache
3. Try incognito/private browsing
4. Check ClientId doesn't have extra spaces

### Infinite Redirect Loop

**Problem**: Keeps redirecting between your app and Google

**Causes**:
- OAuth callback page not handling response correctly
- State mismatch
- Token storage failing

**Solutions**:
1. Check browser console for errors
2. Clear local storage:
   ```javascript
   // In browser console:
   localStorage.clear();
   ```
3. Sign out completely and try again
4. Check OAuthCallback.razor is working

### Stuck on "Completing Sign In..."

**Problem**: OAuth callback page shows "Completing Sign In..." forever

**Check**:
1. Browser console for errors
2. Network tab for failed API calls
3. Token exchange request status

**Solutions**:
1. Verify Client Secret is correct
2. Check system time is accurate
3. Verify redirect URI matches exactly
4. Check firewall isn't blocking Google APIs

### "Invalid State" Error

**Problem**: State parameter doesn't match

**Causes**:
- Browser storage cleared between steps
- Multiple sign-in attempts overlapping

**Solutions**:
1. Sign out completely
2. Clear browser storage
3. Sign in again
4. Don't open multiple sign-in windows

---

## Token & API Issues

### "401 Unauthorized" When Fetching Events

**Problem**: Authenticated but can't fetch events

**Causes**:
- Access token expired and refresh failed
- Token not being sent with request
- Calendar API not enabled

**Solutions**:
1. Check access token is being sent:
   - Open browser DevTools (F12)
   - Go to Network tab
   - Look for Calendar API requests
   - Check Authorization header

2. Sign out and sign in again

3. Verify Calendar API is enabled

### Events Not Loading After Sign In

**Problem**: Sign in succeeds but no events appear

**Check**:
1. Browser console for API errors
2. Network tab for failed Calendar API requests
3. Calendar IDs in configuration

**Solutions**:
1. Verify Calendar IDs are correct:
   ```json
   "CalendarIds": [
     "youremail@gmail.com",  // Must be exact
     "calendar@group.calendar.google.com"
   ]
   ```

2. Check calendars exist and have events in date range

3. Try different view (Day/Week/Month)

4. Check for API quota errors in Console

### Token Refresh Failing

**Problem**: Access token expires and doesn't refresh

**Symptoms**:
- Works initially, then stops
- Must sign in again frequently

**Causes**:
- No refresh token stored
- Refresh token expired or revoked
- Client Secret incorrect

**Solutions**:
1. Check local storage has refresh token:
   ```javascript
   // In browser console:
   JSON.parse(localStorage.getItem('google_oauth_token'))
   ```

2. Verify Client Secret in configuration

3. Revoke access and sign in again:
   - Go to https://myaccount.google.com/permissions
   - Remove "Family Dashboard"
   - Sign in again to get new tokens

### "Insufficient Permission" Error

**Problem**: API returns permission errors

**Causes**:
- Wrong scope configured
- User denied permission
- Calendar access revoked

**Solutions**:
1. Check OAuth consent screen scope:
   - Should include: `calendar.readonly`

2. Sign in shows correct permission:
   - Should ask to "View your calendars"

3. Revoke and re-authorize:
   - https://myaccount.google.com/permissions
   - Remove app
   - Sign in again

---

## Browser & Network Issues

### CORS Errors

**Problem**: CORS errors in browser console

**Note**: OAuth flow shouldn't have CORS issues (it uses redirects, not AJAX)

**If you see CORS errors**:
1. You might be using wrong authentication method
2. Check you're not making API calls before authentication
3. Verify OAuth flow is being used (not API key)

### Cookies/Storage Blocked

**Problem**: Browser blocking cookies or local storage

**Symptoms**:
- Can't stay signed in
- Sign in succeeds but immediately forgets

**Solutions**:
1. Check browser settings allow cookies
2. Check browser allows local storage
3. Disable privacy extensions temporarily
4. Try different browser

### Network/Firewall Issues

**Problem**: Can't connect to Google services

**Check**:
1. Can you access https://accounts.google.com?
2. Can you access https://www.googleapis.com?
3. Corporate firewall blocking?

**Solutions**:
1. Check firewall/proxy settings
2. Try on different network
3. Whitelist Google domains

---

## Configuration Problems

### Port Number Wrong

**Problem**: App runs on different port than configured

**Solutions**:
1. Check launchSettings.json for port:
   ```json
   "applicationUrl": "https://localhost:7104"
   ```

2. Update redirect URI to match actual port

3. Add all possible ports to Google Console:
   ```
   https://localhost:5001/oauth-callback
   https://localhost:7104/oauth-callback
   ```

### Multiple Environments

**Problem**: Configuration works in development but not production

**Solutions**:
1. Create separate OAuth clients for each environment:
   - Development: localhost redirect URI
   - Production: production domain redirect URI

2. Use environment-specific config files:
   - `appsettings.Development.json`
   - `appsettings.Production.json`

3. Update redirect URIs for each environment

### JSON Configuration Errors

**Problem**: Configuration not loading

**Check**:
1. JSON is valid (use a validator)
2. Commas, quotes, brackets correct
3. No trailing commas

**Common mistakes**:
```json
{
  "GoogleOAuth": {
    "ClientId": "abc123",
    "ClientSecret": "xyz789",  // ? No comma after last item
  }  // ? Extra comma before closing brace
}
```

Correct:
```json
{
  "GoogleOAuth": {
    "ClientId": "abc123",
    "ClientSecret": "xyz789"
  }
}
```

---

## Debugging Tips

### Enable Verbose Logging

Add to Program.cs:
```csharp
builder.Logging.SetMinimumLevel(LogLevel.Debug);
```

### Check Browser Console

1. Press F12
2. Go to Console tab
3. Look for errors (red text)
4. Look for warnings (yellow text)

### Check Network Requests

1. Press F12
2. Go to Network tab
3. Filter by "calendar" or "oauth"
4. Look at request/response details
5. Check status codes (200 = good, 400/401 = error)

### Check Local Storage

```javascript
// In browser console:
// View stored token
console.log(localStorage.getItem('google_oauth_token'));

// View parsed token
console.log(JSON.parse(localStorage.getItem('google_oauth_token')));

// Clear storage if needed
localStorage.clear();
```

### Test OAuth Flow Manually

1. Copy the auth URL from console logs
2. Paste in browser manually
3. Complete flow
4. Check redirect URL for code parameter

---

## Still Having Issues?

### Information to Gather

When asking for help, provide:
1. Exact error message
2. Browser console logs
3. Network tab showing failed requests
4. Steps to reproduce
5. What you've already tried

### Resources

- Google OAuth Documentation: https://developers.google.com/identity/protocols/oauth2
- Google Calendar API: https://developers.google.com/calendar/api/v3/reference
- OAuth 2.0 Playground: https://developers.google.com/oauthplayground/

### Common Solution: Start Fresh

If all else fails:
1. Revoke app access: https://myaccount.google.com/permissions
2. Clear browser storage: `localStorage.clear()`
3. Delete and recreate OAuth client in Google Console
4. Update configuration with new credentials
5. Try sign in again

---

## Prevention Best Practices

### During Development
- Save credentials immediately when created
- Test sign-in flow immediately after setup
- Don't commit credentials to Git
- Use environment variables for secrets
- Test sign-out and re-sign-in

### For Production
- Use separate OAuth client for production
- Store secrets in secure vault
- Monitor Google Cloud Console for errors
- Set up error logging/alerts
- Test with multiple users

### Regular Maintenance
- Monitor OAuth consent screen approval status
- Check Google Cloud Console for quota warnings
- Review authorized domains
- Rotate credentials periodically
- Keep documentation updated

---

**Most issues can be resolved by**:
1. Verifying redirect URI matches exactly
2. Checking Client ID and Secret are correct
3. Clearing browser storage and trying again
4. Checking browser console for specific errors

Good luck! ??
