using System.Text.Json.Serialization;

namespace FamilyDashboard.Api.Models.Weather;

public sealed class OpenMeteoForecastResponse
{
    [JsonPropertyName("utc_offset_seconds")]
    public int UtcOffsetSeconds { get; set; }

    [JsonPropertyName("current")]
    public OpenMeteoCurrentResponse? Current { get; set; }

    [JsonPropertyName("hourly")]
    public OpenMeteoHourlyResponse? Hourly { get; set; }

    [JsonPropertyName("daily")]
    public OpenMeteoDailyResponse? Daily { get; set; }
}

public sealed class OpenMeteoCurrentResponse
{
    [JsonPropertyName("time")]
    public string? Time { get; set; }

    [JsonPropertyName("temperature_2m")]
    public double? Temperature2M { get; set; }

    [JsonPropertyName("apparent_temperature")]
    public double? ApparentTemperature { get; set; }

    [JsonPropertyName("weather_code")]
    public int? WeatherCode { get; set; }

    [JsonPropertyName("relative_humidity_2m")]
    public double? RelativeHumidity2M { get; set; }

    [JsonPropertyName("precipitation")]
    public double? Precipitation { get; set; }

    [JsonPropertyName("wind_speed_10m")]
    public double? WindSpeed10M { get; set; }

    [JsonPropertyName("wind_direction_10m")]
    public int? WindDirection10M { get; set; }
}

public sealed class OpenMeteoHourlyResponse
{
    [JsonPropertyName("time")]
    public string[]? Time { get; set; }

    [JsonPropertyName("temperature_2m")]
    public double[]? Temperature2M { get; set; }

    [JsonPropertyName("apparent_temperature")]
    public double[]? ApparentTemperature { get; set; }

    [JsonPropertyName("precipitation_probability")]
    public double[]? PrecipitationProbability { get; set; }

    [JsonPropertyName("precipitation")]
    public double[]? Precipitation { get; set; }

    [JsonPropertyName("weather_code")]
    public int[]? WeatherCode { get; set; }
}

public sealed class OpenMeteoDailyResponse
{
    [JsonPropertyName("time")]
    public string[]? Time { get; set; }

    [JsonPropertyName("sunrise")]
    public string[]? Sunrise { get; set; }

    [JsonPropertyName("sunset")]
    public string[]? Sunset { get; set; }

    [JsonPropertyName("temperature_2m_max")]
    public double[]? Temperature2MMax { get; set; }

    [JsonPropertyName("temperature_2m_min")]
    public double[]? Temperature2MMin { get; set; }

    [JsonPropertyName("weather_code")]
    public int[]? WeatherCode { get; set; }

    [JsonPropertyName("precipitation_probability_max")]
    public double[]? PrecipitationProbabilityMax { get; set; }
}
