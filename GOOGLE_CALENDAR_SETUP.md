# Google Calendar OAuth 2.0 Setup Guide

This guide will walk you through setting up OAuth 2.0 authentication for accessing your **private** Google Calendars in the Family Dashboard.

## Why OAuth 2.0?

OAuth 2.0 allows the application to access your private calendars securely without making them public. Users sign in with their Google account, and the app receives temporary access tokens to read calendar data.

## Overview

The setup process involves:
1. Creating a Google Cloud Project
2. Enabling Google Calendar API
3. Configuring OAuth consent screen
4. Creating OAuth 2.0 credentials
5. Updating your application configuration
6. Testing the authentication flow

---

## Step 1: Create a Google Cloud Project

1. Go to the [Google Cloud Console](https://console.cloud.google.com/)
2. Sign in with your Google account (the one that has the calendars you want to access)
3. Click on the **project dropdown** at the top of the page
4. Click **"New Project"**
5. Enter a project name (e.g., "Family Dashboard")
6. Click **"Create"**
7. Wait for the project to be created (~15-30 seconds)
8. **Important**: Make sure your new project is selected in the dropdown

---

## Step 2: Enable Google Calendar API

1. In the Google Cloud Console, with your project selected
2. Click the **hamburger menu** (?) in the top-left
3. Navigate to **"APIs & Services"** ? **"Library"**
4. Search for **"Google Calendar API"**
5. Click on **"Google Calendar API"** in the results
6. Click the **"Enable"** button
7. Wait for the API to be enabled

---

## Step 3: Configure OAuth Consent Screen

This screen is what users see when they sign in with Google.

### 3.1 Basic Information

1. Go to **"APIs & Services"** ? **"OAuth consent screen"**
2. Select **"External"** user type
   - Choose "External" even for personal use (allows any Google account)
3. Click **"Create"**

### 3.2 App Information

Fill in the required fields:

- **App name**: `Family Dashboard` (or your preferred name)
- **User support email**: Select your email from the dropdown
- **App logo**: (Optional) Upload a logo if desired
- **Application home page**: (Optional) Leave blank for now
- **Application privacy policy**: (Optional) Leave blank for now
- **Application terms of service**: (Optional) Leave blank for now
- **Authorized domains**: (Optional) Leave blank for localhost development

Under **Developer contact information**:
- **Email addresses**: Enter your email address

Click **"Save and Continue"**

### 3.3 Scopes

1. Click **"Add or Remove Scopes"**
2. In the filter box, search for **"calendar"**
3. Check the box for:
   - `https://www.googleapis.com/auth/calendar.readonly` (View your calendars)
4. Click **"Update"** at the bottom
5. Click **"Save and Continue"**

### 3.4 Test Users (Optional but Recommended)

During development, you may want to add test users:

1. Click **"+ Add Users"**
2. Enter the email addresses that should be able to sign in (including your own)
3. Click **"Add"**
4. Click **"Save and Continue"**

### 3.5 Summary

1. Review your OAuth consent screen configuration
2. Click **"Back to Dashboard"**

---

## Step 4: Create OAuth 2.0 Credentials

Now we'll create the Client ID and Client Secret.

### 4.1 Create Credentials

1. Go to **"APIs & Services"** ? **"Credentials"**
2. Click **"+ CREATE CREDENTIALS"** at the top
3. Select **"OAuth client ID"**

### 4.2 Configure OAuth Client

1. **Application type**: Select **"Web application"**
2. **Name**: `Family Dashboard Web Client` (or your preferred name)

### 4.3 Authorized Redirect URIs

This is **critical** - the redirect URI must match exactly what's in your code.

**For Development:**
1. Click **"+ Add URI"** under "Authorized redirect URIs"
2. Enter: `https://localhost:7104/oauth-callback`
   - This must match your development URL exactly
   - Include `/oauth-callback` at the end

**For Production:**
When you deploy to production, you'll need to add your production URL:
- Example: `https://yourdomain.com/oauth-callback`

### 4.4 Create Client

1. Click **"Create"**
2. A modal will appear with your credentials

### 4.5 Save Your Credentials

**Important**: Copy these values immediately!

- **Client ID**: Looks like `790478070066-abc123xyz.apps.googleusercontent.com`
- **Client secret**: Looks like `GOCSPX-ABC123XYZ`

You can also download the JSON file for safekeeping.

Click **"OK"** to close the modal.

---

## Step 5: Configure Your Application

### 5.1 Update Configuration File

1. Open `FamilyDashboard.Blazor\wwwroot\appsettings.Development.json`
2. Update the `GoogleOAuth` section:

```json
{
  "GoogleOAuth": {
    "ClientId": "YOUR_CLIENT_ID_HERE",
    "ClientSecret": "YOUR_CLIENT_SECRET_HERE",
    "RedirectUri": "https://localhost:7104/oauth-callback",
    "CalendarIds": [
      "youremail@gmail.com",
      "anothercalendar@group.calendar.google.com"
    ]
  }
}
```

### 5.2 Configuration Details

**ClientId**: Paste your OAuth Client ID from Step 4.5
- Format: `123456789-abc...xyz.apps.googleusercontent.com`

**ClientSecret**: Paste your OAuth Client Secret from Step 4.5
- Format: `GOCSPX-...`

**RedirectUri**: Must match exactly what you entered in Step 4.3
- Development: `https://localhost:7104/oauth-callback`
- Production: `https://yourdomain.com/oauth-callback`

**CalendarIds**: List of calendar IDs to access
- Your primary calendar is usually your Gmail address
- Find calendar IDs in Google Calendar ? Settings ? Integrate calendar
- See `CALENDAR_IDS_REFERENCE.md` for your existing calendar IDs

### 5.3 Example Configuration

```json
{
  "FamilyName": "Smith",
  "Weather": {
    "Latitude": 0.0,
    "Longitude": 0.0,
    "LocationName": "New York"
  },
  "GoogleOAuth": {
    "ClientId": "YOUR CLIENT ID",
    "ClientSecret": "ENTER YOURS HERE",
    "RedirectUri": "https://localhost:7104/oauth-callback",
    "CalendarIds": [
      "john.smith@gmail.com",
      "jane.smith@gmail.com",
      "en.usa#holiday@group.v.calendar.google.com"
    ]
  }
}
```

---

## Step 6: Test the OAuth Flow

### 6.1 Run Your Application

```bash
dotnet run --project FamilyDashboard.Blazor
```

### 6.2 Sign In Process

1. Navigate to your application in the browser
2. Go to the Calendar tile
3. You should see a **"Sign in with Google"** button
4. Click the button
5. You'll be redirected to Google's sign-in page
6. Sign in with your Google account
7. Google will ask for permission to access your calendars
8. Review the permissions and click **"Allow"** or **"Continue"**
9. You'll be redirected back to your application
10. The calendar should now display your events!

### 6.3 What to Expect

**First Time Sign In:**
- Google shows a warning if your app is not verified (this is normal for development)
- Click "Advanced" ? "Go to Family Dashboard (unsafe)" to continue
- This warning won't appear for published apps

**Permission Screen:**
- Google will ask to "View your calendars"
- This is the calendar.readonly scope we configured
- Click "Allow" to grant access

**After Sign In:**
- You'll be redirected to your app
- Your events will load automatically
- Your authentication persists across page refreshes (stored in browser local storage)

---

## Step 7: Verify Everything Works

### 7.1 Check Calendar Display

- [ ] Events from all configured calendars are visible
- [ ] Day, Week, and Month views all work
- [ ] Click events to see details
- [ ] Navigation (previous/next) works

### 7.2 Test Sign Out

- [ ] Click "Sign Out" button
- [ ] Calendar switches to "Sign in with Google" screen
- [ ] Sign in again to restore access

### 7.3 Test Persistence

- [ ] Refresh the page
- [ ] You should remain signed in
- [ ] Events should load without signing in again

---

## Security Best Practices

### ?? Protect Your Credentials

1. **Never commit secrets to Git**
   - Add `appsettings.Development.json` to `.gitignore`
   - Use environment variables in production

2. **Restrict OAuth Client**
   - Only add necessary redirect URIs
   - Limit to your actual domains

3. **Use HTTPS**
   - OAuth requires HTTPS (localhost is exempt)
   - In production, always use HTTPS

4. **Monitor Usage**
   - Check Google Cloud Console regularly
   - Review OAuth consent logs
   - Set up billing alerts

### ??? Token Security

**Where tokens are stored:**
- Access tokens: Browser local storage
- Refresh tokens: Browser local storage

**Security considerations:**
- Tokens are only accessible to your domain
- Tokens expire after 1 hour (then automatically refresh)
- Users can revoke access at any time via Google Account settings

**To enhance security:**
- Consider implementing server-side token storage (requires backend API)
- Add token encryption (for production)
- Implement automatic sign-out after inactivity

---

## Troubleshooting

### "Redirect URI mismatch" Error

**Problem**: Google shows an error about redirect URI not matching

**Solutions**:
1. Check the redirect URI in Google Cloud Console ? Credentials
2. Must match exactly: `https://localhost:7104/oauth-callback`
3. Check port number (7104 is default, yours may differ)
4. Check for trailing slashes (don't include one)
5. Check protocol (https, not http)

**To find your actual redirect URI**:
- Look at your browser's address bar when running the app
- Should be `https://localhost:XXXX`
- Add `/oauth-callback` to get full redirect URI

### "Access denied" or "Invalid scope" Error

**Problem**: Google denies access or shows scope errors

**Solutions**:
1. Verify Calendar API is enabled in Google Cloud Console
2. Check OAuth consent screen scopes include `calendar.readonly`
3. Try clearing browser cache and cookies
4. Sign out and sign in again

### "App isn't verified" Warning

**Problem**: Google shows a warning screen

**Solution**: This is normal for development!
- Click "Advanced"
- Click "Go to Family Dashboard (unsafe)"
- For production, submit your app for verification (optional)

### No Events Showing After Sign In

**Problem**: Sign in succeeds but no events appear

**Solutions**:
1. Open browser console (F12) and check for errors
2. Verify Calendar IDs in `appsettings.Development.json`
3. Ensure calendars exist and have events
4. Check date range (try changing views)
5. Verify access token is being sent (check network tab)

### Sign In Button Does Nothing

**Problem**: Clicking "Sign in with Google" doesn't redirect

**Solutions**:
1. Check browser console for JavaScript errors
2. Verify `ClientId` in configuration is correct
3. Verify `RedirectUri` in configuration matches Google Cloud Console
4. Try hard refresh (Ctrl+Shift+R)

### Token Expired Errors

**Problem**: Errors about expired tokens

**Solution**: The app should automatically refresh tokens
1. Check browser console for refresh errors
2. Sign out and sign in again
3. Verify `ClientSecret` is correct

### CORS Errors

**Problem**: CORS errors in browser console

**Solution**: This shouldn't happen with OAuth flow
- OAuth redirects don't use CORS
- If you see CORS errors, there may be a different issue
- Check that you're using the OAuth flow, not API key method

---

## Production Deployment

### Update Redirect URI

1. Deploy your application to production
2. Note your production URL (e.g., `https://yourdomain.com`)
3. Go to Google Cloud Console ? Credentials
4. Edit your OAuth client
5. Add production redirect URI: `https://yourdomain.com/oauth-callback`
6. Save changes

### Update Configuration

Create `appsettings.Production.json`:

```json
{
  "GoogleOAuth": {
    "ClientId": "YOUR_CLIENT_ID",
    "ClientSecret": "USE_ENVIRONMENT_VARIABLE_OR_KEY_VAULT",
    "RedirectUri": "https://yourdomain.com/oauth-callback",
    "CalendarIds": [...]
  }
}
```

**Important**: Never commit production secrets to Git!

### Environment Variables (Recommended)

Instead of storing secrets in config files:

```json
{
  "GoogleOAuth": {
    "ClientId": "ENV:GOOGLE_CLIENT_ID",
    "ClientSecret": "ENV:GOOGLE_CLIENT_SECRET",
    "RedirectUri": "https://yourdomain.com/oauth-callback",
    "CalendarIds": [...]
  }
}
```

Then set environment variables on your server.

---

## Understanding OAuth 2.0 Flow

### How It Works

1. **User clicks "Sign in"**
   - App redirects to Google's authorization page
   - URL includes: client_id, redirect_uri, scope, state

2. **User authenticates with Google**
   - User enters Google credentials
   - Google verifies identity

3. **User grants permissions**
   - Google shows what the app wants to access
   - User clicks "Allow"

4. **Google redirects back**
   - Google sends authorization code to your redirect URI
   - Code is temporary (expires in ~10 minutes)

5. **App exchanges code for tokens**
   - App sends code + client_secret to Google
   - Google returns access_token and refresh_token

6. **App uses access token**
   - Included in API requests as Authorization header
   - Valid for ~1 hour

7. **Token refresh** (automatic)
   - When access token expires, app uses refresh_token
   - Gets new access token without user interaction
   - Refresh token is long-lived (until revoked)

### What Gets Stored

**In Browser Local Storage:**
```json
{
  "AccessToken": "ya29.a0AfB...",
  "RefreshToken": "1//0g...",
  "ExpiresAt": "2024-01-15T15:00:00Z",
  "TokenType": "Bearer"
}
```

**Security Note**: This is reasonably secure for client-side apps, but tokens can be accessed by JavaScript on your domain.

---

## Rate Limits & Quotas

Google Calendar API has usage limits:

- **Queries per day**: 1,000,000 (more than enough for personal use)
- **Queries per 100 seconds per user**: 50,000
- **Queries per 100 seconds**: 1,000,000

Your app will typically make:
- 1-3 API calls when loading calendar (one per calendar ID)
- Calls only happen when changing views or refreshing
- Well within free tier limits

---

## Managing Access

### User Perspective

Users can revoke access at any time:
1. Go to [Google Account Permissions](https://myaccount.google.com/permissions)
2. Find "Family Dashboard"
3. Click "Remove Access"

### You Can Also Revoke

From Google Cloud Console:
1. Go to OAuth consent screen
2. View granted access
3. Revoke if needed

---

## Additional Resources

- [Google OAuth 2.0 Documentation](https://developers.google.com/identity/protocols/oauth2)
- [Google Calendar API Reference](https://developers.google.com/calendar/api/v3/reference)
- [OAuth 2.0 Playground](https://developers.google.com/oauthplayground/) (for testing)

---

## Support

### Common Questions

**Q: Can multiple users sign in?**
A: Yes! Each user signs in with their own Google account and sees their own calendars.

**Q: Do I need to verify my app?**
A: Not for personal/family use. Verification is only needed for apps with many users or published to the public.

**Q: How long does authentication last?**
A: Until the user signs out or revokes access. Tokens refresh automatically.

**Q: Can I access calendars from multiple Google accounts?**
A: Not simultaneously. Each browser session is for one Google account. Users would need to sign out and sign in with a different account.

**Q: What happens if I change my Google password?**
A: Access tokens remain valid. For security, you may want to revoke app access and sign in again.

---

## Summary Checklist

- [ ] Created Google Cloud Project
- [ ] Enabled Google Calendar API  
- [ ] Configured OAuth consent screen
- [ ] Created OAuth 2.0 credentials
- [ ] Copied Client ID and Client Secret
- [ ] Updated `appsettings.Development.json`
- [ ] Added redirect URI: `https://localhost:7104/oauth-callback`
- [ ] Added calendar IDs to configuration
- [ ] Tested sign-in flow
- [ ] Verified events display correctly
- [ ] Tested sign-out
- [ ] Tested persistence (refresh page)

**You're all set!** Enjoy secure access to your private Google Calendars! ??
