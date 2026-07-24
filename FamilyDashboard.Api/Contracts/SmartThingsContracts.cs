using FamilyDashboard.Blazor.Models.SmartHome;

namespace FamilyDashboard.Api.Contracts;

public record SmartThingsTokenRequest(string Token);
public record SmartThingsStatusResponse(bool HasToken, bool IsTokenValid);
public record SetSwitchRequest(SmartHomeSwitch SwitchValue);
