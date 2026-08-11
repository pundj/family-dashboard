using FamilyDashboard.Api.Models.Weather;
using FamilyDashboard.Blazor.Models.Weather;
using System.Globalization;

namespace FamilyDashboard.Api.Services;

public static class WeatherResponseMapper
{
    public static WeatherData Map(
        OpenMeteoForecastResponse forecast,
        string? locationName,
        IReadOnlyList<WeatherAlert> alerts,
        bool alertsAvailable,
        string? alertsErrorMessage)
    {
        var offset = TimeSpan.FromSeconds(forecast.UtcOffsetSeconds);
        var daily = MapDaily(forecast.Daily, offset);
        var current = MapCurrent(forecast.Current, offset, daily);
        var hourly = MapHourly(forecast.Hourly, offset, daily);

        return new WeatherData
        {
            LocationName = locationName,
            RetrievedAtUtc = DateTimeOffset.UtcNow,
            CurrentConditions = current,
            HourlyForecast = hourly,
            DailyForecast = daily,
            Alerts = alerts,
            AlertsAvailable = alertsAvailable,
            AlertsErrorMessage = alertsErrorMessage
        };
    }

    public static CurrentWeatherConditions? MapCurrent(OpenMeteoCurrentResponse? current) =>
        MapCurrent(current, TimeSpan.Zero, null);

    public static CurrentWeatherConditions? MapCurrent(
        OpenMeteoCurrentResponse? current,
        TimeSpan offset,
        IReadOnlyList<DailyWeatherForecastEntry>? daily)
    {
        if (current is null)
        {
            return null;
        }

        var isNight = TryParseLocationDateTimeOffset(current.Time, offset, out var time) &&
                      IsNighttime(time, daily);

        return new CurrentWeatherConditions
        {
            Temperature = current.Temperature2M,
            FeelsLikeTemperature = current.ApparentTemperature,
            Condition = WeatherCodeMapper.GetCondition(current.WeatherCode, isNight),
            WeatherCode = current.WeatherCode,
            Icon = WeatherCodeMapper.GetIcon(current.WeatherCode, isNight),
            RelativeHumidity = current.RelativeHumidity2M,
            Precipitation = current.Precipitation,
            WindSpeed = current.WindSpeed10M,
            WindDirection = current.WindDirection10M
        };
    }

    public static IReadOnlyList<HourlyWeatherForecastEntry> MapHourly(OpenMeteoHourlyResponse? hourly, TimeSpan offset, IReadOnlyList<DailyWeatherForecastEntry>? daily = null)
    {
        if (hourly?.Time is null)
        {
            return [];
        }

        var count = hourly.Time.Length;
        var result = new List<HourlyWeatherForecastEntry>(count);
        for (var index = 0; index < count; index++)
        {
            if (!TryParseLocationDateTimeOffset(hourly.Time[index], offset, out var time))
            {
                continue;
            }

            var weatherCode = GetValue(hourly.WeatherCode, index);
            var isNight = IsNighttime(time, daily);
            result.Add(new HourlyWeatherForecastEntry
            {
                Time = time,
                Temperature = GetValue(hourly.Temperature2M, index),
                FeelsLikeTemperature = GetValue(hourly.ApparentTemperature, index),
                PrecipitationProbability = GetValue(hourly.PrecipitationProbability, index),
                PrecipitationAmount = GetValue(hourly.Precipitation, index),
                WeatherCode = weatherCode,
                Condition = WeatherCodeMapper.GetCondition(weatherCode, isNight),
                Icon = WeatherCodeMapper.GetIcon(weatherCode, isNight)
            });
        }

        return result;
    }

    public static IReadOnlyList<DailyWeatherForecastEntry> MapDaily(OpenMeteoDailyResponse? daily, TimeSpan offset)
    {
        if (daily?.Time is null)
        {
            return [];
        }

        var count = daily.Time.Length;
        var result = new List<DailyWeatherForecastEntry>(count);
        for (var index = 0; index < count; index++)
        {
            if (!DateOnly.TryParse(daily.Time[index], CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
            {
                continue;
            }

            var weatherCode = GetValue(daily.WeatherCode, index);
            result.Add(new DailyWeatherForecastEntry
            {
                Date = date,
                Sunrise = TryParseLocationDateTimeOffset(GetValue(daily.Sunrise, index), offset, out var sunrise) ? sunrise : null,
                Sunset = TryParseLocationDateTimeOffset(GetValue(daily.Sunset, index), offset, out var sunset) ? sunset : null,
                HighTemperature = GetValue(daily.Temperature2MMax, index),
                LowTemperature = GetValue(daily.Temperature2MMin, index),
                PrecipitationProbability = GetValue(daily.PrecipitationProbabilityMax, index),
                WeatherCode = weatherCode,
                Condition = WeatherCodeMapper.GetCondition(weatherCode),
                Icon = WeatherCodeMapper.GetIcon(weatherCode)
            });
        }

        return result;
    }

    public static IReadOnlyList<WeatherAlert> MapAlerts(NwsAlertResponse? response)
    {
        if (response?.Features is null || response.Features.Length == 0)
        {
            return [];
        }

        var alerts = new List<WeatherAlert>();
        foreach (var feature in response.Features)
        {
            var properties = feature.Properties;
            if (properties is null)
            {
                continue;
            }

            if (!string.IsNullOrWhiteSpace(properties.Status) &&
                !string.Equals(properties.Status, "Actual", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            alerts.Add(new WeatherAlert
            {
                EventTitle = properties.Event,
                Headline = properties.Headline,
                Severity = properties.Severity,
                Urgency = properties.Urgency,
                Certainty = properties.Certainty,
                EffectiveTime = properties.Effective,
                ExpirationTime = properties.Expires,
                Description = properties.Description,
                Instructions = properties.Instruction,
                Area = properties.AreaDesc
            });
        }

        return alerts;
    }

    private static T? GetValue<T>(IReadOnlyList<T>? values, int index)
    {
        if (values is null || index < 0 || index >= values.Count)
        {
            return default;
        }

        return values[index];
    }

    private static bool IsNighttime(DateTimeOffset time, IReadOnlyList<DailyWeatherForecastEntry>? daily)
    {
        if (daily is null || daily.Count == 0)
        {
            return false;
        }

        var date = DateOnly.FromDateTime(time.DateTime);
        var day = daily.FirstOrDefault(entry => entry.Date == date);
        if (day is null)
        {
            return false;
        }

        if (day.Sunrise is not null && day.Sunset is not null)
        {
            return time < day.Sunrise || time >= day.Sunset;
        }

        return time.Hour is < 6 or >= 20;
    }

    private static bool TryParseLocationDateTimeOffset(string? value, TimeSpan offset, out DateTimeOffset result)
    {
        result = default;
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        if (!DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out var localDateTime))
        {
            return false;
        }

        result = new DateTimeOffset(DateTime.SpecifyKind(localDateTime, DateTimeKind.Unspecified), offset);
        return true;
    }
}
