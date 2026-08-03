namespace FamilyDashboard.Blazor.Models.Calendar;

public class CalendarPreferences
{
    public WeekStartDay WeekStart { get; set; } = WeekStartDay.Sunday;
    public Dictionary<string, CalendarSettings> Calendars { get; set; } = new();
}

public class CalendarSettings
{
    public string CalendarId { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Color { get; set; } = "#3788d8"; // Default blue
    public bool IsVisible { get; set; } = true;
    public bool IncludeInNextEvent { get; set; } = true;
}
