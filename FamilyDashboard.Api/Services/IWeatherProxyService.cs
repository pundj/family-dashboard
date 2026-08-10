using FamilyDashboard.Blazor.Models.Weather;

namespace FamilyDashboard.Api.Services;

public interface IWeatherProxyService
{
    Task<WeatherData> GetWeatherAsync(double latitude, double longitude, string? locationName, CancellationToken cancellationToken = default);
}
