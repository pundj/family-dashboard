namespace FamilyDashboard.Blazor.Models.Weather;

public static class WeatherForecastPaging
{
    public const int PageSize = 5;

    public static int ClampStartIndex(int startIndex, int totalCount)
    {
        if (totalCount <= 0)
        {
            return 0;
        }

        var maxStartIndex = ((totalCount - 1) / PageSize) * PageSize;
        return Math.Clamp(startIndex, 0, maxStartIndex);
    }

    public static int GetNextStartIndex(int currentStartIndex, int totalCount) =>
        ClampStartIndex(currentStartIndex + PageSize, totalCount);

    public static int GetPreviousStartIndex(int currentStartIndex, int totalCount) =>
        ClampStartIndex(currentStartIndex - PageSize, totalCount);

    public static bool CanGoPrevious(int currentStartIndex) => currentStartIndex > 0;

    public static bool CanGoNext(int currentStartIndex, int totalCount) => currentStartIndex + PageSize < totalCount;

    public static IReadOnlyList<HourlyWeatherForecastEntry> GetPage(IReadOnlyList<HourlyWeatherForecastEntry> hourlyForecast, int startIndex)
    {
        if (hourlyForecast.Count == 0)
        {
            return [];
        }

        var clampedStartIndex = ClampStartIndex(startIndex, hourlyForecast.Count);
        var count = Math.Min(PageSize, hourlyForecast.Count - clampedStartIndex);
        return hourlyForecast.Skip(clampedStartIndex).Take(count).ToArray();
    }
}
