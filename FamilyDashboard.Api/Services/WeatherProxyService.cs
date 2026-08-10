using FamilyDashboard.Api.Models.Weather;
using FamilyDashboard.Blazor.Models.Weather;
using Microsoft.Extensions.Caching.Memory;
using System.Globalization;

namespace FamilyDashboard.Api.Services;

public sealed class WeatherProxyService(
    IHttpClientFactory httpClientFactory,
    IMemoryCache memoryCache,
    ILogger<WeatherProxyService> logger)
    : IWeatherProxyService
{
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);
    private readonly SemaphoreSlim _cacheLock = new(1, 1);

    public async Task<WeatherData> GetWeatherAsync(double latitude, double longitude, string? locationName, CancellationToken cancellationToken = default)
    {
        var cacheKey = BuildCacheKey(latitude, longitude, locationName);
        if (memoryCache.TryGetValue(cacheKey, out WeatherData? cachedWeather) && cachedWeather is not null)
        {
            return cachedWeather;
        }

        await _cacheLock.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (memoryCache.TryGetValue(cacheKey, out cachedWeather) && cachedWeather is not null)
            {
                return cachedWeather;
            }

            var forecast = await GetOpenMeteoForecastAsync(latitude, longitude, cancellationToken).ConfigureAwait(false);
            var (alerts, alertsAvailable, alertsErrorMessage) = await GetNwsAlertsAsync(latitude, longitude, cancellationToken).ConfigureAwait(false);
            var weatherData = WeatherResponseMapper.Map(forecast, locationName, alerts, alertsAvailable, alertsErrorMessage);

            memoryCache.Set(cacheKey, weatherData, CacheDuration);
            return weatherData;
        }
        catch (Exception ex) when (ex is not OperationCanceledException && memoryCache.TryGetValue(cacheKey, out cachedWeather) && cachedWeather is not null)
        {
            logger.LogWarning(ex, "Weather proxy request failed for {Latitude}, {Longitude}; returning cached data.", latitude, longitude);
            return cachedWeather;
        }
        finally
        {
            _cacheLock.Release();
        }
    }

    private async Task<OpenMeteoForecastResponse> GetOpenMeteoForecastAsync(double latitude, double longitude, CancellationToken cancellationToken)
    {
        var client = CreateOpenMeteoClient();
        var requestUri = string.Create(CultureInfo.InvariantCulture,
            $"forecast?latitude={latitude}&longitude={longitude}&current=temperature_2m,apparent_temperature,relative_humidity_2m,precipitation,weather_code,wind_speed_10m,wind_direction_10m&hourly=temperature_2m,apparent_temperature,precipitation_probability,precipitation,weather_code&daily=weather_code,temperature_2m_max,temperature_2m_min,precipitation_probability_max,sunrise,sunset&temperature_unit=fahrenheit&windspeed_unit=mph&precipitation_unit=inch&forecast_days=10&forecast_hours=48&timezone=auto");

        var response = await client.GetAsync(requestUri, cancellationToken).ConfigureAwait(false);
        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"Open-Meteo request failed with status {(int)response.StatusCode}.");
        }

        return await response.Content.ReadFromJsonAsync<OpenMeteoForecastResponse>(cancellationToken: cancellationToken).ConfigureAwait(false)
            ?? throw new InvalidOperationException("Open-Meteo response was empty.");
    }

    private async Task<(IReadOnlyList<WeatherAlert> alerts, bool alertsAvailable, string? alertsErrorMessage)> GetNwsAlertsAsync(double latitude, double longitude, CancellationToken cancellationToken)
    {
        try
        {
            var client = CreateNwsClient();
            using var request = new HttpRequestMessage(HttpMethod.Get, $"alerts/active?point={latitude.ToString(CultureInfo.InvariantCulture)},{longitude.ToString(CultureInfo.InvariantCulture)}");

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                logger.LogWarning("NWS alerts request failed with status {StatusCode}.", response.StatusCode);
                return ([], false, "Weather alerts are temporarily unavailable.");
            }

            var nwsResponse = await response.Content.ReadFromJsonAsync<NwsAlertResponse>(cancellationToken: cancellationToken).ConfigureAwait(false);
            return (WeatherResponseMapper.MapAlerts(nwsResponse), true, null);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "NWS alerts request failed.");
            return ([], false, "Weather alerts are temporarily unavailable.");
        }
    }

    private HttpClient CreateOpenMeteoClient()
    {
        var client = httpClientFactory.CreateClient("OpenMeteo");
        return client;
    }

    private HttpClient CreateNwsClient()
    {
        var client = httpClientFactory.CreateClient("Nws");
        return client;
    }

    private static string BuildCacheKey(double latitude, double longitude, string? locationName) =>
        string.Create(CultureInfo.InvariantCulture, $"weather:{latitude:0.0000}:{longitude:0.0000}:{locationName?.Trim().ToLowerInvariant() ?? string.Empty}");
}
