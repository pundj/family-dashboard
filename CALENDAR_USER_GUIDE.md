# Calendar User Guide

## Features

### View Types

The calendar supports three different view types:

1. **Day View** - Shows a detailed list of events for a single day
2. **Week View** - Shows events across 7 days in a grid format
3. **Month View** - Shows events across an entire month in a traditional calendar layout

### Navigation

- **Previous/Next buttons (?/?)**: Navigate to the previous or next time period
- **Today button**: Jump back to the current date
- **View selector**: Switch between Day, Week, and Month views

### Week View Options

When in Week view, you can choose the start day:
- **Sunday Start**: Week runs Sunday through Saturday
- **Monday Start**: Week runs Monday through Sunday

### Event Details

- In **Day View**: Events show title, time range, and location
- In **Week View**: Events show title and time within each day cell
- In **Month View**: Events show title only (up to 3 events per day, with "+X more" indicator)

Click on any event in any view to see full details:
- Title
- Start date/time
- End date/time
- Location (if available)
- Description (if available)

### Event Types

- **Timed Events**: Show specific start and end times
- **All-Day Events**: Display as "All Day" without specific times

## Tips

1. **Multiple Calendars**: Events from all configured calendars are merged and displayed together
2. **Color Coding**: All events currently use the same blue color, but you can customize this by modifying the CSS in the view components
3. **Responsive**: The calendar is designed to work on different screen sizes
4. **Real-time**: Events are fetched from Google Calendar when you load the page or switch views

## Keyboard Shortcuts

Currently not implemented, but could be added for:
- Arrow keys to navigate between periods
- 'T' to go to today
- 'D', 'W', 'M' to switch views

## Customization

You can customize the appearance by modifying the CSS in:
- `Calendar.razor` - Main calendar and modal styles
- `DayView.razor` - Day view specific styles
- `WeekView.razor` - Week view specific styles
- `MonthView.razor` - Month view specific styles

## Known Limitations

1. **Public Calendars Only**: With API key authentication, only public calendars can be accessed
2. **Read-Only**: You cannot create, edit, or delete events from this interface
3. **No Recurring Events**: Recurring events are shown as individual instances
4. **No Reminders**: Calendar reminders are not displayed or triggered
5. **Limited Event Details**: Only shows title, time, location, and description

## Future Enhancements

Potential features that could be added:
- Filter events by calendar
- Create/edit events
- Event search
- Export to ICS
- Keyboard navigation
- Print view
- Event categories/tags
- Recurring event details
