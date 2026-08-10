using FamilyDashboard.Blazor.Models.Weather;
using System.Globalization;
using System.Net.Http.Json;

namespace FamilyDashboard.Blazor.Services;

public class WeatherService(HttpClient httpClient, IConfiguration configuration, ILogger<WeatherService> logger)
    : IWeatherService
{
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);
    private readonly SemaphoreSlim _cacheLock = new(1, 1);
    private WeatherData? _cachedWeatherData;
    private DateTimeOffset _cachedAtUtc = DateTimeOffset.MinValue;

    public async Task<WeatherData> GetWeatherAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTimeOffset.UtcNow;
        if (_cachedWeatherData is not null && now - _cachedAtUtc < CacheDuration)
        {
            return _cachedWeatherData;
        }

        await _cacheLock.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (_cachedWeatherData is not null && DateTimeOffset.UtcNow - _cachedAtUtc < CacheDuration)
            {
                return _cachedWeatherData;
            }

            var (latitude, longitude, locationName) = GetWeatherConfiguration();
            var requestUri = $"api/weather?latitude={latitude.ToString(CultureInfo.InvariantCulture)}&longitude={longitude.ToString(CultureInfo.InvariantCulture)}&locationName={Uri.EscapeDataString(locationName ?? string.Empty)}";

            var weatherData = await httpClient.GetFromJsonAsync<WeatherData>(requestUri, cancellationToken).ConfigureAwait(false)
                ?? throw new InvalidOperationException("Weather service returned no data.");

            _cachedWeatherData = weatherData;
            _cachedAtUtc = DateTimeOffset.UtcNow;
            return weatherData;
        }
        catch (Exception ex) when (ex is not OperationCanceledException && _cachedWeatherData is not null)
        {
            logger.LogWarning(ex, "Weather data request failed; returning cached data.");
            return _cachedWeatherData;
        }
        finally
        {
            _cacheLock.Release();
        }
    }

    private (double latitude, double longitude, string? locationName) GetWeatherConfiguration()
    {
        var latitude = configuration.GetValue<double?>("Weather:Latitude");
        var longitude = configuration.GetValue<double?>("Weather:Longitude");
        var locationName = configuration.GetValue<string?>("Weather:LocationName");

        if (latitude is null || longitude is null)
        {
            throw new InvalidOperationException("Weather configuration requires Latitude and Longitude.");
        }

        return (latitude.Value, longitude.Value, locationName);
    }
}
