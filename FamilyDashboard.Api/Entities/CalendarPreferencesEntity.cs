namespace FamilyDashboard.Api.Entities;

public class CalendarPreferencesEntity
{
    public string PreferenceKey { get; set; } = "default";
    public string PreferencesJson { get; set; } = "{}";
    public DateTimeOffset UpdatedUtc { get; set; }
}
