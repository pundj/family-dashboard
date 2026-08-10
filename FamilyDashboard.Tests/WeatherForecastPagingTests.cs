using FamilyDashboard.Blazor.Models.Weather;

namespace FamilyDashboard.Tests;

public class WeatherForecastPagingTests
{
    [Fact]
    public void GetPage_ReturnsFirstFiveEntriesByDefault()
    {
        var hourly = Enumerable.Range(1, 10)
            .Select(index => new HourlyWeatherForecastEntry { Time = new DateTimeOffset(2026, 1, index, 0, 0, 0, TimeSpan.Zero) })
            .ToList();

        var page = WeatherForecastPaging.GetPage(hourly, 0);

        Assert.Equal(5, page.Count);
        Assert.Equal(1, page[0].Time.Day);
        Assert.Equal(5, page[^1].Time.Day);
    }

    [Fact]
    public void PagingMovesInSixHourChunks()
    {
        Assert.Equal(6, WeatherForecastPaging.GetNextStartIndex(0, 10));
        Assert.Equal(0, WeatherForecastPaging.GetPreviousStartIndex(0, 10));
        Assert.Equal(6, WeatherForecastPaging.GetPreviousStartIndex(12, 10));
        Assert.True(WeatherForecastPaging.CanGoNext(0, 10));
        Assert.True(WeatherForecastPaging.CanGoPrevious(6));
    }
}
