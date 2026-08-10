namespace FamilyDashboard.Blazor.Models.Weather;

public static class WeatherCodeMapper
{
    public static WeatherConditionPresentation GetPresentation(int? weatherCode, bool isNight = false) => weatherCode switch
    {
        0 => isNight
            ? new WeatherConditionPresentation("Clear", "🌙")
            : new WeatherConditionPresentation("Sunny", "☀️"),
        1 => new WeatherConditionPresentation("Mostly Sunny", "🌤️"),
        2 => new WeatherConditionPresentation("Partly Cloudy", "⛅"),
        3 => new WeatherConditionPresentation("Cloudy", "☁️"),
        45 or 48 => new WeatherConditionPresentation("Foggy", "🌫️"),
        51 or 53 or 55 => new WeatherConditionPresentation("Drizzle", "🌦️"),
        56 or 57 => new WeatherConditionPresentation("Freezing Drizzle", "🌧️"),
        61 or 63 or 65 => new WeatherConditionPresentation("Rain", "🌧️"),
        66 or 67 => new WeatherConditionPresentation("Freezing Rain", "🌧️"),
        71 or 73 or 75 or 77 => new WeatherConditionPresentation("Snow", "🌨️"),
        80 or 81 or 82 => new WeatherConditionPresentation("Rain Showers", "🌦️"),
        85 or 86 => new WeatherConditionPresentation("Snow Showers", "🌨️"),
        95 or 96 or 99 => new WeatherConditionPresentation("Thunderstorm", "⛈️"),
        _ => new WeatherConditionPresentation("Unknown", "🌡️")
    };

    public static string GetCondition(int? weatherCode, bool isNight = false) => GetPresentation(weatherCode, isNight).Condition;

    public static string GetIcon(int? weatherCode, bool isNight = false) => GetPresentation(weatherCode, isNight).Icon;
}
