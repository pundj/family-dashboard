using FamilyDashboard.Blazor.Models.Weather;

namespace FamilyDashboard.Tests;

public class WeatherCodeMapperTests
{
    [Theory]
    [InlineData(0, "Sunny", "☀️")]
    [InlineData(1, "Mostly Sunny", "🌤️")]
    [InlineData(2, "Partly Cloudy", "⛅")]
    [InlineData(3, "Cloudy", "☁️")]
    [InlineData(45, "Foggy", "🌫️")]
    [InlineData(61, "Rain", "🌧️")]
    [InlineData(71, "Snow", "🌨️")]
    [InlineData(95, "Thunderstorm", "⛈️")]
    public void GetPresentation_ReturnsExpectedConditionAndIcon(int code, string condition, string icon)
    {
        var presentation = WeatherCodeMapper.GetPresentation(code);

        Assert.Equal(condition, presentation.Condition);
        Assert.Equal(icon, presentation.Icon);
    }

    [Fact]
    public void UnknownCode_ReturnsUnknownPresentation()
    {
        var presentation = WeatherCodeMapper.GetPresentation(999);

        Assert.Equal("Unknown", presentation.Condition);
        Assert.Equal("🌡️", presentation.Icon);
    }
}
