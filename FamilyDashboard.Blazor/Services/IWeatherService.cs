using FamilyDashboard.Blazor.Models.Weather;

namespace FamilyDashboard.Blazor.Services;

public interface IWeatherService
{
    Task<WeatherData> GetWeatherAsync(CancellationToken cancellationToken = default);
}
