using FamilyDashboard.Blazor.Models.Calendar;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Net.Http.Json;
using System.Text.Json;

namespace FamilyDashboard.Blazor.Services;

public class GoogleAuthService : IGoogleAuthService
{
    private readonly IConfiguration _configuration;
    private readonly IJSRuntime _jsRuntime;
    private readonly NavigationManager _navigationManager;
    private readonly HttpClient _httpClient;
    private readonly GoogleOAuthSettings _settings;
    private OAuthToken? _currentToken;

    private const string TOKEN_STORAGE_KEY = "google_oauth_token";
    private const string AUTH_ENDPOINT = "https://accounts.google.com/o/oauth2/v2/auth";
    private const string TOKEN_ENDPOINT = "https://oauth2.googleapis.com/token";
    private const string CALENDAR_SCOPE = "https://www.googleapis.com/auth/calendar.readonly";

    public event Action? OnAuthStateChanged;

    public GoogleAuthService(
        IConfiguration configuration,
        IJSRuntime jsRuntime,
        NavigationManager navigationManager,
        IHttpClientFactory httpClientFactory)
    {
        _configuration = configuration;
        _jsRuntime = jsRuntime;
        _navigationManager = navigationManager;
        _httpClient = httpClientFactory.CreateClient();
        
        var settings = _configuration.GetSection("GoogleOAuth").Get<GoogleOAuthSettings>();
        _settings = settings ?? new GoogleOAuthSettings();
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        await LoadTokenFromStorageAsync();
        
        if (_currentToken == null)
            return false;

        if (_currentToken.IsExpired)
        {
            return await RefreshTokenAsync();
        }

        return true;
    }

    public async Task<string?> GetAccessTokenAsync()
    {
        if (!await IsAuthenticatedAsync())
            return null;

        return _currentToken?.AccessToken;
    }

    public async Task InitiateAuthFlowAsync()
    {
        var state = Guid.NewGuid().ToString();
        await StoreStateAsync(state);

        var authUrl = $"{AUTH_ENDPOINT}" +
            $"?client_id={Uri.EscapeDataString(_settings.ClientId)}" +
            $"&redirect_uri={Uri.EscapeDataString(_settings.RedirectUri)}" +
            $"&response_type=code" +
            $"&scope={Uri.EscapeDataString(CALENDAR_SCOPE)}" +
            $"&state={Uri.EscapeDataString(state)}" +
            $"&access_type=offline" +
            $"&prompt=consent";

        _navigationManager.NavigateTo(authUrl, true);
    }

    public async Task<bool> HandleAuthCallbackAsync(string code)
    {
        try
        {
            var tokenRequest = new Dictionary<string, string>
            {
                { "code", code },
                { "client_id", _settings.ClientId },
                { "client_secret", _settings.ClientSecret },
                { "redirect_uri", _settings.RedirectUri },
                { "grant_type", "authorization_code" }
            };

            var response = await _httpClient.PostAsync(
                TOKEN_ENDPOINT,
                new FormUrlEncodedContent(tokenRequest));

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Token exchange failed: {error}");
                return false;
            }

            var tokenResponse = await response.Content.ReadFromJsonAsync<JsonElement>();
            
            _currentToken = new OAuthToken
            {
                AccessToken = tokenResponse.GetProperty("access_token").GetString() ?? "",
                RefreshToken = tokenResponse.TryGetProperty("refresh_token", out var rt) 
                    ? rt.GetString() ?? "" 
                    : "",
                ExpiresAt = DateTime.UtcNow.AddSeconds(
                    tokenResponse.GetProperty("expires_in").GetInt32()),
                TokenType = tokenResponse.GetProperty("token_type").GetString() ?? "Bearer"
            };

            await SaveTokenToStorageAsync();
            OnAuthStateChanged?.Invoke();
            
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error handling auth callback: {ex.Message}");
            return false;
        }
    }

    public async Task SignOutAsync()
    {
        _currentToken = null;
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", TOKEN_STORAGE_KEY);
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "oauth_state");
        OnAuthStateChanged?.Invoke();
    }

    private async Task<bool> RefreshTokenAsync()
    {
        if (_currentToken?.RefreshToken == null)
            return false;

        try
        {
            var refreshRequest = new Dictionary<string, string>
            {
                { "refresh_token", _currentToken.RefreshToken },
                { "client_id", _settings.ClientId },
                { "client_secret", _settings.ClientSecret },
                { "grant_type", "refresh_token" }
            };

            var response = await _httpClient.PostAsync(
                TOKEN_ENDPOINT,
                new FormUrlEncodedContent(refreshRequest));

            if (!response.IsSuccessStatusCode)
            {
                await SignOutAsync();
                return false;
            }

            var tokenResponse = await response.Content.ReadFromJsonAsync<JsonElement>();
            
            _currentToken.AccessToken = tokenResponse.GetProperty("access_token").GetString() ?? "";
            _currentToken.ExpiresAt = DateTime.UtcNow.AddSeconds(
                tokenResponse.GetProperty("expires_in").GetInt32());

            await SaveTokenToStorageAsync();
            OnAuthStateChanged?.Invoke();
            
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error refreshing token: {ex.Message}");
            await SignOutAsync();
            return false;
        }
    }

    private async Task LoadTokenFromStorageAsync()
    {
        try
        {
            var tokenJson = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", TOKEN_STORAGE_KEY);
            
            if (!string.IsNullOrEmpty(tokenJson))
            {
                _currentToken = JsonSerializer.Deserialize<OAuthToken>(tokenJson);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading token from storage: {ex.Message}");
        }
    }

    private async Task SaveTokenToStorageAsync()
    {
        try
        {
            if (_currentToken != null)
            {
                var tokenJson = JsonSerializer.Serialize(_currentToken);
                await _jsRuntime.InvokeVoidAsync("localStorage.setItem", TOKEN_STORAGE_KEY, tokenJson);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving token to storage: {ex.Message}");
        }
    }

    private async Task StoreStateAsync(string state)
    {
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "oauth_state", state);
    }
}
