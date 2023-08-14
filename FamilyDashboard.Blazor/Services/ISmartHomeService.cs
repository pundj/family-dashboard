using FamilyDashboard.Blazor.Models.SmartHome;

namespace FamilyDashboard.Blazor.Services;

public interface ISmartHomeService
{
    Task<GetSmartHomeDevicesResponse?> GetDevicesAsync();
    Task<SmartThingsDeviceViewModel?> GetDeviceAsync(string? deviceId);
    Task SetSwitchAsync(string? deviceId, SmartHomeSwitch switchValue);
}
