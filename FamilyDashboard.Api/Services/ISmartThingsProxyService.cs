using FamilyDashboard.Blazor.Models.SmartHome;

namespace FamilyDashboard.Api.Services;

public interface ISmartThingsProxyService
{
    Task<bool> ValidateTokenAsync(string token);
    Task<GetSmartHomeDevicesResponse> GetDevicesAsync(string token);
    Task<SmartThingsDeviceViewModel> GetDeviceAsync(string token, string deviceId);
    Task SetSwitchAsync(string token, string deviceId, SmartHomeSwitch switchValue);
}
