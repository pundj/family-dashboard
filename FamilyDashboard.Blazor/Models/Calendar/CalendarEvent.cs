namespace FamilyDashboard.Blazor.Models.Calendar;

public class CalendarEvent
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string? Location { get; set; }
    public bool IsAllDay { get; set; }
    public string CalendarId { get; set; } = string.Empty;
    public string Color { get; set; } = "#3788d8"; // Default blue
}
