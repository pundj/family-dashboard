# Calendar IDs Found in Your Old Embedded Calendar

Based on your previous embedded calendar configuration, here are the Calendar IDs you were using. You can add these to your `appsettings.Development.json` file:

## Identified Calendar IDs:

```json
{
  "GoogleCalendar": {
    "ApiKey": "YOUR_API_KEY_HERE",
    "CalendarIds": [
      "john.smith@gmail.com",
      "jane.smith@gmail.com",
      "en.usa#holiday@group.v.calendar.google.com"
    ]
  }
}
```

## Calendar Types:

- **Personal calendars**: `john.smith@gmail.com`, `jane.smith@gmail.com`
- **Contacts**: `addressbook#contacts@group.v.calendar.google.com`
- **Group/Shared calendars**: IDs ending with `@group.calendar.google.com`
- **Imported calendars**: IDs ending with `@import.calendar.google.com`
- **Holiday calendar**: `en.usa#holiday@group.v.calendar.google.com`

## Notes:

1. You can include all of these or just select the ones you want to display
2. All calendars will need to be made public (or you can implement OAuth 2.0 for private access)
3. If any calendar is deleted or you no longer have access, the app will skip it and continue with the others
4. Events from all calendars will be merged and displayed together, sorted by time

## Quick Start Configuration:

For a minimal setup, you could start with just the main personal calendars:

```json
{
  "GoogleCalendar": {
    "ApiKey": "YOUR_API_KEY_HERE",
    "CalendarIds": [
      "john.smith@gmail.com",
      "jane.smith@gmail.com",
      "en.usa#holiday@group.v.calendar.google.com"
    ]
  }
}
```

Then add more calendars as needed.
