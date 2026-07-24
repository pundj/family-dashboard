using FamilyDashboard.Blazor.Models.SmartHome;
using System.Net;
using System.Net.Http.Json;

namespace FamilyDashboard.Blazor.Services;

public class SmartThingsService(HttpClient httpClient) : ISmartHomeService
{
    public async Task<SmartHomeUserStatus> GetUserStatusAsync()
    {
        var authStatus = await httpClient.GetFromJsonAsync<AuthStatusResponse>("api/auth/status");
        if (authStatus?.IsAuthenticated != true)
            return new SmartHomeUserStatus { IsAuthenticating = false };

        var tokenStatus = await httpClient.GetFromJsonAsync<SmartThingsStatusResponse>("api/me/smartthings/status");

        return new SmartHomeUserStatus
        {
            IsAuthenticated = true,
            IsAuthenticating = false,
            UserName = authStatus.UserName,
            HasToken = tokenStatus?.HasToken == true,
            IsTokenValid = tokenStatus?.IsTokenValid == true
        };
    }

    public async Task<AuthActionResult> RegisterAsync(string userName, string password)
    {
        return await SendAuthAsync("api/auth/register", new AuthRequest(userName, password));
    }

    public async Task<AuthActionResult> LoginAsync(string userName, string password)
    {
        return await SendAuthAsync("api/auth/login", new AuthRequest(userName, password));
    }

    public async Task LogoutAsync()
    {
        await httpClient.PostAsync("api/auth/logout", content: null);
    }

    public async Task<AuthActionResult> SaveTokenAsync(string token)
    {
        var response = await httpClient.PostAsJsonAsync("api/me/smartthings/token", new SmartThingsTokenRequest(token));
        if (response.IsSuccessStatusCode)
            return new AuthActionResult { Succeeded = true };

        if (response.StatusCode == HttpStatusCode.BadRequest)
            return new AuthActionResult { Succeeded = false, Message = "Token validation failed. Please verify the token and try again." };

        return new AuthActionResult { Succeeded = false, Message = "Unable to save token." };
    }

    public async Task RemoveTokenAsync()
    {
        await httpClient.DeleteAsync("api/me/smartthings/token");
    }

    public async Task<GetSmartHomeDevicesResponse?> GetDevicesAsync()
    {
        return await httpClient.GetFromJsonAsync<GetSmartHomeDevicesResponse>("api/me/smartthings/devices");
    }

    public async Task<SmartThingsDeviceViewModel?> GetDeviceAsync(string? deviceId)
    {
        if (string.IsNullOrWhiteSpace(deviceId))
            throw new ArgumentNullException(nameof(deviceId));

        return await httpClient.GetFromJsonAsync<SmartThingsDeviceViewModel>($"api/me/smartthings/devices/{deviceId}");
    }

    public async Task SetSwitchAsync(string? deviceId, SmartHomeSwitch switchValue)
    {
        if (string.IsNullOrWhiteSpace(deviceId))
            throw new ArgumentNullException(nameof(deviceId));
        if (switchValue == SmartHomeSwitch.Unknown)
            throw new ArgumentException("switchValue cannot be Unknown", nameof(switchValue));

        var response = await httpClient.PostAsJsonAsync(
            $"api/me/smartthings/devices/{deviceId}/switch",
            new SetSwitchRequest(switchValue));

        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"Failed to set switch: {response.StatusCode} - {content}");
        }
    }

    private async Task<AuthActionResult> SendAuthAsync(string uri, AuthRequest request)
    {
        var response = await httpClient.PostAsJsonAsync(uri, request);
        if (response.IsSuccessStatusCode)
            return new AuthActionResult { Succeeded = true };

        var payload = await response.Content.ReadFromJsonAsync<AuthActionResponse>();
        return new AuthActionResult { Succeeded = false, Message = payload?.Message ?? "Request failed." };
    }

    private record AuthRequest(string UserName, string Password);
    private record AuthStatusResponse(bool IsAuthenticated, string? UserName);
    private record AuthActionResponse(bool Succeeded, string? Message);
    private record SmartThingsTokenRequest(string Token);
    private record SmartThingsStatusResponse(bool HasToken, bool IsTokenValid);
    private record SetSwitchRequest(SmartHomeSwitch SwitchValue);
}
