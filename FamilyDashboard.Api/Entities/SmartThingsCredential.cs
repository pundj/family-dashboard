namespace FamilyDashboard.Api.Entities;

public class SmartThingsCredential
{
    public string UserId { get; set; } = string.Empty;
    public string ProtectedToken { get; set; } = string.Empty;
    public DateTimeOffset UpdatedUtc { get; set; }
}
