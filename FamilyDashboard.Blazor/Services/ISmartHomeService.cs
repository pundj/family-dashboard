using FamilyDashboard.Blazor.Models.SmartHome;

namespace FamilyDashboard.Blazor.Services;

public interface ISmartHomeService
{
    Task<SmartHomeUserStatus> GetUserStatusAsync();
    Task<AuthActionResult> RegisterAsync(string userName, string password);
    Task<AuthActionResult> LoginAsync(string userName, string password);
    Task LogoutAsync();
    Task<AuthActionResult> SaveTokenAsync(string token);
    Task RemoveTokenAsync();

    Task<GetSmartHomeDevicesResponse?> GetDevicesAsync();
    Task<SmartThingsDeviceViewModel?> GetDeviceAsync(string? deviceId);
    Task SetSwitchAsync(string? deviceId, SmartHomeSwitch switchValue);
}

public class SmartHomeUserStatus
{
    public bool IsAuthenticated { get; set; }
    public bool IsAuthenticating { get; set; } = true;
    public string? UserName { get; set; }
    public bool HasToken { get; set; }
    public bool IsTokenValid { get; set; }
}

public class AuthActionResult
{
    public bool Succeeded { get; set; }
    public string? Message { get; set; }
}
