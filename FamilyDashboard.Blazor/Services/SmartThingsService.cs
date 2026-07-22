using FamilyDashboard.Blazor.Models.SmartHome;
using System.Net;
using System.Net.Http.Json;

namespace FamilyDashboard.Blazor.Services;

public class SmartThingsService : ISmartHomeService
{
    private readonly HttpClient _httpClient;

    public SmartThingsService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<SmartHomeUserStatus> GetUserStatusAsync()
    {
        var authStatus = await _httpClient.GetFromJsonAsync<AuthStatusResponse>("api/auth/status");
        if (authStatus?.IsAuthenticated != true)
            return new SmartHomeUserStatus();

        var tokenStatus = await _httpClient.GetFromJsonAsync<SmartThingsStatusResponse>("api/me/smarthings/status");

        return new SmartHomeUserStatus
        {
            IsAuthenticated = true,
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
        await _httpClient.PostAsync("api/auth/logout", content: null);
    }

    public async Task<AuthActionResult> SaveTokenAsync(string token)
    {
        var response = await _httpClient.PostAsJsonAsync("api/me/smarthings/token", new SmartThingsTokenRequest(token));
        if (response.IsSuccessStatusCode)
            return new AuthActionResult { Succeeded = true };

        if (response.StatusCode == HttpStatusCode.BadRequest)
            return new AuthActionResult { Succeeded = false, Message = "Token validation failed. Please verify the token and try again." };

        return new AuthActionResult { Succeeded = false, Message = "Unable to save token." };
    }

    public async Task RemoveTokenAsync()
    {
        await _httpClient.DeleteAsync("api/me/smarthings/token");
    }

    public async Task<GetSmartHomeDevicesResponse?> GetDevicesAsync()
    {
        return await _httpClient.GetFromJsonAsync<GetSmartHomeDevicesResponse>("api/me/smarthings/devices");
    }

    public async Task<SmartThingsDeviceViewModel?> GetDeviceAsync(string? deviceId)
    {
        if (string.IsNullOrWhiteSpace(deviceId))
            throw new ArgumentNullException(nameof(deviceId));

        return await _httpClient.GetFromJsonAsync<SmartThingsDeviceViewModel>($"api/me/smarthings/devices/{deviceId}");
    }

    public async Task SetSwitchAsync(string? deviceId, SmartHomeSwitch switchValue)
    {
        if (string.IsNullOrWhiteSpace(deviceId))
            throw new ArgumentNullException(nameof(deviceId));
        if (switchValue == SmartHomeSwitch.Unknown)
            throw new ArgumentException("switchValue cannot be Unknown", nameof(switchValue));

        var response = await _httpClient.PostAsJsonAsync(
            $"api/me/smarthings/devices/{deviceId}/switch",
            new SetSwitchRequest(switchValue));

        response.EnsureSuccessStatusCode();
    }

    private async Task<AuthActionResult> SendAuthAsync(string uri, AuthRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync(uri, request);
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
