namespace FamilyDashboard.Blazor.Models.Weather;

public sealed record WeatherData
{
    public string? LocationName { get; init; }
    public DateTimeOffset RetrievedAtUtc { get; init; } = DateTimeOffset.UtcNow;
    public CurrentWeatherConditions? CurrentConditions { get; init; }
    public IReadOnlyList<HourlyWeatherForecastEntry> HourlyForecast { get; init; } = [];
    public IReadOnlyList<DailyWeatherForecastEntry> DailyForecast { get; init; } = [];
    public IReadOnlyList<WeatherAlert> Alerts { get; init; } = [];
    public bool AlertsAvailable { get; init; } = true;
    public string? AlertsErrorMessage { get; init; }
}

public sealed record CurrentWeatherConditions
{
    public double? Temperature { get; init; }
    public double? FeelsLikeTemperature { get; init; }
    public string? Condition { get; init; }
    public int? WeatherCode { get; init; }
    public string? Icon { get; init; }
    public double? RelativeHumidity { get; init; }
    public double? Precipitation { get; init; }
    public double? PrecipitationProbability { get; init; }
    public double? WindSpeed { get; init; }
    public int? WindDirection { get; init; }
}

public sealed record HourlyWeatherForecastEntry
{
    public DateTimeOffset Time { get; init; }
    public double? Temperature { get; init; }
    public double? FeelsLikeTemperature { get; init; }
    public double? PrecipitationProbability { get; init; }
    public double? PrecipitationAmount { get; init; }
    public string? Condition { get; init; }
    public int? WeatherCode { get; init; }
    public string? Icon { get; init; }
}

public sealed record DailyWeatherForecastEntry
{
    public DateOnly Date { get; init; }
    public DateTimeOffset? Sunrise { get; init; }
    public DateTimeOffset? Sunset { get; init; }
    public double? HighTemperature { get; init; }
    public double? LowTemperature { get; init; }
    public double? PrecipitationProbability { get; init; }
    public string? Condition { get; init; }
    public int? WeatherCode { get; init; }
    public string? Icon { get; init; }
}

public sealed record WeatherAlert
{
    public string? EventTitle { get; init; }
    public string? Headline { get; init; }
    public string? Severity { get; init; }
    public string? Urgency { get; init; }
    public string? Certainty { get; init; }
    public DateTimeOffset? EffectiveTime { get; init; }
    public DateTimeOffset? ExpirationTime { get; init; }
    public string? Description { get; init; }
    public string? Instructions { get; init; }
    public string? Area { get; init; }
}

public sealed record WeatherConditionPresentation(string Condition, string Icon);
