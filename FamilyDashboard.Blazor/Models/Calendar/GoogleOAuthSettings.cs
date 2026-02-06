namespace FamilyDashboard.Blazor.Models.Calendar;

public class GoogleOAuthSettings
{
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string RedirectUri { get; set; } = string.Empty;
    public List<string> CalendarIds { get; set; } = new();
    public Dictionary<string, string> CalendarNames { get; set; } = new();
}

