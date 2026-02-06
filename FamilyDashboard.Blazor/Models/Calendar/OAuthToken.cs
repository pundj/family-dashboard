namespace FamilyDashboard.Blazor.Models.Calendar;

public class OAuthToken
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public string TokenType { get; set; } = "Bearer";
    
    public bool IsExpired => DateTime.UtcNow >= ExpiresAt.AddMinutes(-5); // Refresh 5 min before expiry
}
