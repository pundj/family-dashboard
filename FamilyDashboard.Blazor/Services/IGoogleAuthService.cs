using FamilyDashboard.Blazor.Models.Calendar;

namespace FamilyDashboard.Blazor.Services;

public interface IGoogleAuthService
{
    Task<bool> IsAuthenticatedAsync();
    Task<string?> GetAccessTokenAsync();
    Task InitiateAuthFlowAsync();
    Task<bool> HandleAuthCallbackAsync(string code);
    Task SignOutAsync();
    event Action? OnAuthStateChanged;
}
