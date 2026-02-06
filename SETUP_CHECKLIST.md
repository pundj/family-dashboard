# Google Calendar Integration - Setup Checklist

## ? Pre-Setup Checklist

- [ ] Have a Google account with calendars
- [ ] Know which calendars you want to display
- [ ] Have access to Google Cloud Console
- [ ] Have your project ready to run

## ?? Google Cloud Console Setup

### Step 1: Create Project
- [ ] Go to https://console.cloud.google.com/
- [ ] Click project dropdown at top
- [ ] Click "New Project"
- [ ] Enter project name: "Family Dashboard"
- [ ] Click "Create"
- [ ] Wait for project creation (15-30 seconds)
- [ ] Select your new project from dropdown

### Step 2: Enable API
- [ ] Open hamburger menu (?)
- [ ] Navigate to "APIs & Services" ? "Library"
- [ ] Search for "Google Calendar API"
- [ ] Click on "Google Calendar API"
- [ ] Click "Enable" button
- [ ] Wait for API to be enabled

## ?? Google Calendar Configuration

For each calendar you want to display:

### Calendar 1: [Your Primary Calendar]
- [ ] Go to https://calendar.google.com/
- [ ] Find calendar in left sidebar
- [ ] Click ? (three dots) next to it
- [ ] Click "Settings and sharing"
- [ ] Under "Access permissions for events":
  - [ ] Check ? "Make available to public"
  - [ ] Select "See all event details"
- [ ] Scroll to "Integrate calendar" section
- [ ] Copy "Calendar ID": ___________________________
- [ ] Save this Calendar ID

### Calendar 2: [Second Calendar]
- [ ] Go to https://calendar.google.com/
- [ ] Find calendar in left sidebar
- [ ] Click ? (three dots) next to it
- [ ] Click "Settings and sharing"
- [ ] Under "Access permissions for events":
  - [ ] Check ? "Make available to public"
  - [ ] Select "See all event details"
- [ ] Scroll to "Integrate calendar" section
- [ ] Copy "Calendar ID": ___________________________
- [ ] Save this Calendar ID

### Calendar 3+: [Additional Calendars]
- [ ] Repeat above steps for each additional calendar
- [ ] Calendar ID: ___________________________
- [ ] Calendar ID: ___________________________
- [ ] Calendar ID: ___________________________

## ?? Application Configuration

### Update Configuration File
- [ ] Open `FamilyDashboard.Blazor\wwwroot\appsettings.Development.json`
- [ ] Find the `GoogleCalendar` section
- [ ] Update `"CalendarIds"` array with your calendar IDs:

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

### Verify Configuration
- [ ] Each Calendar ID is on its own line
- [ ] Calendar IDs are comma-separated (except the last one)
- [ ] All quotes and brackets are properly closed
- [ ] JSON is valid (use a JSON validator if unsure)

## ?? Troubleshooting

If you encounter issues, check:

### No Events Showing
- [ ] Open browser console (F12)
- [ ] Look for error messages
- [ ] Verify API key is correct
- [ ] Verify Calendar IDs are correct
- [ ] Verify calendars are public
- [ ] Check Google Cloud Console for API errors

### API Errors
- [ ] Go to Google Cloud Console
- [ ] Navigate to "APIs & Services" ? "Dashboard"
- [ ] Check if Google Calendar API is enabled
- [ ] Check quota usage and errors
- [ ] Verify API key restrictions

### Permission Denied
- [ ] Go back to Google Calendar
- [ ] Verify each calendar is set to public
- [ ] Check "Access permissions" settings
- [ ] Try accessing calendar URL directly

### Build Errors
- [ ] Verify all files were created
- [ ] Check for typos in configuration
- [ ] Run `dotnet clean` then `dotnet build`
- [ ] Check that service is registered in Program.cs

## ?? Reference Documentation

Have these files handy:
- [ ] `QUICK_START.md` - Fast setup guide
- [ ] `GOOGLE_CALENDAR_SETUP.md` - Detailed setup
- [ ] `CALENDAR_IDS_REFERENCE.md` - Your calendar IDs
- [ ] `CALENDAR_USER_GUIDE.md` - Feature usage
- [ ] `ARCHITECTURE.md` - Technical details


**Congratulations! Your Google Calendar integration is complete!**

---

## Quick Reference

**API Key Location**: Google Cloud Console ? APIs & Services ? Credentials

**Calendar Settings**: Google Calendar ? Calendar ? ? Settings and sharing

**Config File**: `FamilyDashboard.Blazor\wwwroot\appsettings.Development.json`

**Build Command**: `dotnet build`

**Run Command**: `dotnet run --project FamilyDashboard.Blazor`

**Browser Console**: Press F12
