namespace FamilyDashboard.Blazor.Services;

public interface IWeatherForecastService
{
    Task<string> GetWeatherForecastBase64ImageStringAsync();
}
