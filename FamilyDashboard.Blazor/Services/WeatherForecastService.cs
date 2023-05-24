namespace FamilyDashboard.Blazor.Services;

public class WeatherForecastService : IWeatherForecastService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public WeatherForecastService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClient = httpClientFactory.CreateClient("WeatherForecast");
        _configuration = configuration;
    }

    public async Task<string> GetWeatherForecastBase64ImageStringAsync()
    {
        var locale = _configuration["Locale"];

        if (string.IsNullOrEmpty(locale))
            throw new Exception("Locale app setting is required for Weather Forecast");
        var formatOptions = "_background=212529";
        var responseStream = await _httpClient.GetStreamAsync($"https://wttr.in/{locale}{formatOptions}.png");
        var memoryStream = new MemoryStream();
        await responseStream.CopyToAsync(memoryStream);
        return Convert.ToBase64String(memoryStream.ToArray());
    }
}
