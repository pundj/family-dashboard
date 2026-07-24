namespace FamilyDashboard.Api.Services;

public interface ISmartThingsCredentialStore
{
    Task<bool> HasTokenAsync(string userId);
    Task<string?> GetTokenAsync(string userId);
    Task SaveTokenAsync(string userId, string token);
    Task RemoveTokenAsync(string userId);
}
