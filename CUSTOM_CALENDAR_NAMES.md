# Custom Calendar Names Configuration

## ?? Overview

You can now specify custom display names for your calendars directly in the `appsettings.Development.json` file. This gives you complete control over how calendar names appear in the UI.

## ?? Configuration

### Simple Configuration (Auto-Generated Names)

If you don't specify custom names, the system will auto-generate them:

```json
{
  "GoogleOAuth": {
    "ClientId": "your-client-id",
    "ClientSecret": "your-client-secret",
    "RedirectUri": "https://localhost:7104/oauth-callback",
    "CalendarIds": [
      "john.smith@gmail.com",
      "jane.smith@gmail.com"
    ]
  }
}
```

**Auto-generated names:**
- `john.smith@gmail.com` ? "john.smith"
- `jane.smith@gmail.com` ? "jane.smith"
- OAuth client ID ? "Primary Calendar"
- `@import.calendar.google.com` ? "Imported Calendar"
- `@group.calendar.google.com` ? "Shared Calendar"
- Holiday calendars ? "Holidays (US)"

### Custom Configuration (Your Own Names)

Add a `CalendarNames` dictionary to specify custom names:

```json
{
  "GoogleOAuth": {
    "ClientId": "your-client-id",
    "ClientSecret": "your-client-secret",
    "RedirectUri": "https://localhost:7104/oauth-callback",
    "CalendarIds": [
      "john.smith@gmail.com",
      "jane.smith@gmail.com",
      "abc123...@import.calendar.google.com",
      "xyz789...@group.calendar.google.com"
    ],
    "CalendarNames": {
      "john.smith@gmail.com": "John's Calendar",
      "jane.smith@gmail.com": "Jane's Work Schedule",
      "abc123...@import.calendar.google.com": "Jimmy's Sports Calendar",
      "xyz789...@group.calendar.google.com": "Jimmy's School Calendar"
    }
  }
}
```

## ?? How It Works

### Priority Order

The system determines display names in this order:

1. **Custom Name** (from `CalendarNames` in appsettings) ? **Highest Priority**
2. **Auto-Generated Name** (based on calendar ID pattern)
3. **Calendar ID** (as fallback)

### Example

```json
"CalendarIds": [
  "john@gmail.com"
],
"CalendarNames": {
  "john@gmail.com": "Family Calendar"
}
```

**Result:** Shows "Family Calendar" instead of "john"

## ?? Complete Example

```json
{
  "FamilyName": "Smith",
  "Weather": {
    "Latitude": 0.0,
    "Longitude": 0.0,
    "LocationName": "New York"
  },
  "GoogleOAuth": {
    "ClientId": "123456789-abc.apps.googleusercontent.com",
    "ClientSecret": "GOCSPX-...",
    "RedirectUri": "https://localhost:7104/oauth-callback",
    "CalendarIds": [
      "john.smith@gmail.com",
      "jane.smith@gmail.com",
      "123456789-abc.apps.googleusercontent.com",
      "en.usa#holiday@group.v.calendar.google.com",
      "abc123xyz@import.calendar.google.com",
      "def456uvw@group.calendar.google.com"
    ],
    "CalendarNames": {
      "john.smith@gmail.com": "John's Personal",
      "jane.smith@gmail.com": "Jane's Work",
      "123456789-abc.apps.googleusercontent.com": "Family Shared",
      "en.usa#holiday@group.v.calendar.google.com": "US Holidays",
      "abc123xyz@import.calendar.google.com": "St. Mary's Church",
      "def456uvw@group.calendar.google.com": "Lincoln Elementary"
    }
  }
}
```

## ?? Use Cases

### Family Calendars
```json
"CalendarNames": {
  "dad@gmail.com": "Dad's Schedule",
  "mom@gmail.com": "Mom's Calendar",
  "primary-calendar-id": "Family Events"
}
```

### Work & Personal
```json
"CalendarNames": {
  "personal@gmail.com": "Personal",
  "work@company.com": "Work Schedule",
  "team-calendar-id": "Team Meetings"
}
```

### Community Organizations
```json
"CalendarNames": {
  "church-import-id": "Church Events",
  "school-group-id": "School Activities",
  "sports-group-id": "Soccer Team"
}
```

## ?? Where Names Appear

Custom calendar names are displayed in:

1. **Calendar Settings Modal** (?? button)
   - Shows in the list of calendars
   - Next to color picker and visibility checkbox

2. **Event Details Modal**
   - Shows as a colored badge
   - Indicates which calendar an event belongs to

## ? Benefits

### Before (Auto-Generated)
```
Settings:
? [blue] abc123xyz@import.calendar.google.com
? [red]  def456uvw@group.calendar.google.com
```

### After (Custom Names)
```
Settings:
? [blue] St. Mary's Church
? [red]  Lincoln Elementary
```

## ?? Configuration Tips

### Naming Best Practices

? **Do:**
- Keep names short and descriptive
- Use names that make sense to all family members
- Be consistent with capitalization

? **Don't:**
- Use very long names (they may get cut off)
- Include special characters that might cause issues
- Use identical names for different calendars

### Finding Calendar IDs

1. Go to your Google Calendar settings
2. Select the calendar
3. Scroll to "Integrate calendar"
4. Copy the "Calendar ID"
5. Add it to both `CalendarIds` and `CalendarNames`

### Optional Names

You don't have to name every calendar:

```json
"CalendarIds": [
  "john@gmail.com",
  "jane@gmail.com",
  "shared@gmail.com"
],
"CalendarNames": {
  "shared@gmail.com": "Family Calendar"
}
```

**Result:**
- `john@gmail.com` ? "john" (auto-generated)
- `jane@gmail.com` ? "jane" (auto-generated)
- `shared@gmail.com` ? "Family Calendar" (custom)

## ?? Quick Start

1. **Copy your calendar IDs** from Google Calendar settings
2. **Add them to `CalendarIds` array**
3. **Add a `CalendarNames` object** (optional)
4. **Map each ID to a friendly name**
5. **Save and refresh** your dashboard

### Minimal Example

```json
{
  "GoogleOAuth": {
    "CalendarIds": ["you@gmail.com"],
    "CalendarNames": {
      "you@gmail.com": "My Calendar"
    }
  }
}
```

## ?? Related Documentation

- **Setup Guide**: `QUICK_START.md`
- **Calendar Features**: `CALENDAR_COLORS_FEATURES.md`
- **User Guide**: `CALENDAR_COLORS_USER_GUIDE.md`
- **Calendar IDs Reference**: `CALENDAR_IDS_REFERENCE.md`

## ?? Examples from Real Users

### Simple Family Setup
```json
"CalendarNames": {
  "dad@gmail.com": "Dad",
  "mom@gmail.com": "Mom",
  "kids@gmail.com": "Kids"
}
```

### Detailed Organization
```json
"CalendarNames": {
  "personal@gmail.com": "?? Personal",
  "work@company.com": "?? Work",
  "gym@import.com": "?? Gym Classes",
  "school@group.com": "?? School Events"
}
```

### Using Emojis
```json
"CalendarNames": {
  "work@gmail.com": "?? Work",
  "family@gmail.com": "??????????? Family",
  "sports@gmail.com": "? Sports",
  "church@import.com": "? Church"
}
```

---

**Now you have full control over your calendar names!** ????
