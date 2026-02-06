namespace FamilyDashboard.Blazor.Models.Calendar;

public class GoogleCalendarSettings
{
    public string ApiKey { get; set; } = string.Empty;
    public List<string> CalendarIds { get; set; } = new();
}
