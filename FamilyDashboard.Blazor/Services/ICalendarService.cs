using FamilyDashboard.Blazor.Models.Calendar;

namespace FamilyDashboard.Blazor.Services;

public interface ICalendarService
{
    Task<List<CalendarEvent>> GetEventsAsync(DateTime startDate, DateTime endDate);
    Task<CalendarEvent?> GetNextEventAsync();
}
