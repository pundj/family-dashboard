using FamilyDashboard.Api.Models.Weather;
using FamilyDashboard.Api.Services;

namespace FamilyDashboard.Tests;

public class WeatherResponseMapperTests
{
    [Fact]
    public void MapCurrent_MapsCurrentConditions()
    {
        var current = new OpenMeteoCurrentResponse
        {
            Temperature2M = 87,
            ApparentTemperature = 91,
            WeatherCode = 0,
            RelativeHumidity2M = 54,
            Precipitation = 0,
            WindSpeed10M = 12,
            WindDirection10M = 225
        };

        var mapped = WeatherResponseMapper.MapCurrent(current);

        Assert.NotNull(mapped);
        Assert.Equal(87, mapped!.Temperature);
        Assert.Equal(91, mapped.FeelsLikeTemperature);
        Assert.Equal(0, mapped.WeatherCode);
        Assert.Equal(54, mapped.RelativeHumidity);
        Assert.Equal(0, mapped.Precipitation);
        Assert.Equal(12, mapped.WindSpeed);
        Assert.Equal(225, mapped.WindDirection);
        Assert.Equal("Sunny", mapped.Condition);
        Assert.Equal("☀️", mapped.Icon);
    }

    [Fact]
    public void MapHourly_MapsHourlyForecast()
    {
        var hourly = new OpenMeteoHourlyResponse
        {
            Time = ["2026-01-01T00:00", "2026-01-01T01:00"],
            Temperature2M = [87, 86],
            ApparentTemperature = [90, 88],
            PrecipitationProbability = [10, 20],
            Precipitation = [0.01, 0.02],
            WeatherCode = [1, 61]
        };

        var mapped = WeatherResponseMapper.MapHourly(hourly, TimeSpan.Zero);

        Assert.Collection(mapped,
            first =>
            {
                Assert.Equal(87, first.Temperature);
                Assert.Equal(90, first.FeelsLikeTemperature);
                Assert.Equal(10, first.PrecipitationProbability);
                Assert.Equal(0.01, first.PrecipitationAmount);
                Assert.Equal("Mostly Sunny", first.Condition);
                Assert.Equal("🌤️", first.Icon);
            },
            second =>
            {
                Assert.Equal(86, second.Temperature);
                Assert.Equal(88, second.FeelsLikeTemperature);
                Assert.Equal(20, second.PrecipitationProbability);
                Assert.Equal(0.02, second.PrecipitationAmount);
                Assert.Equal("Rain", second.Condition);
                Assert.Equal("🌧️", second.Icon);
            });
    }

    [Fact]
    public void MapDaily_MapsDailyForecast()
    {
        var daily = new OpenMeteoDailyResponse
        {
            Time = ["2026-01-01", "2026-01-02"],
            Temperature2MMax = [91, 88],
            Temperature2MMin = [72, 69],
            WeatherCode = [2, 95],
            PrecipitationProbabilityMax = [15, 80]
        };

        var mapped = WeatherResponseMapper.MapDaily(daily, TimeSpan.Zero);

        Assert.Collection(mapped,
            first =>
            {
                Assert.Equal(new DateOnly(2026, 1, 1), first.Date);
                Assert.Equal(91, first.HighTemperature);
                Assert.Equal(72, first.LowTemperature);
                Assert.Equal(15, first.PrecipitationProbability);
                Assert.Equal("Partly Cloudy", first.Condition);
                Assert.Equal("⛅", first.Icon);
            },
            second =>
            {
                Assert.Equal(new DateOnly(2026, 1, 2), second.Date);
                Assert.Equal(88, second.HighTemperature);
                Assert.Equal(69, second.LowTemperature);
                Assert.Equal(80, second.PrecipitationProbability);
                Assert.Equal("Thunderstorm", second.Condition);
                Assert.Equal("⛈️", second.Icon);
            });
    }

    [Fact]
    public void MapAlerts_ReturnsEmptyForEmptyResponse()
    {
        var mapped = WeatherResponseMapper.MapAlerts(new NwsAlertResponse { Features = [] });

        Assert.Empty(mapped);
    }

    [Fact]
    public void MapAlerts_MapsOptionalFieldsAndSkipsNonActualAlerts()
    {
        var response = new NwsAlertResponse
        {
            Features =
            [
                new NwsAlertFeature
                {
                    Properties = new NwsAlertProperties
                    {
                        Event = "Heat Advisory",
                        Headline = "High temperatures expected",
                        Severity = "Severe",
                        Urgency = "Expected",
                        Certainty = "Likely",
                        Effective = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero),
                        Expires = new DateTimeOffset(2026, 1, 1, 20, 0, 0, TimeSpan.Zero),
                        Description = "Stay hydrated.",
                        Instruction = "Drink water.",
                        AreaDesc = "Jefferson County",
                        Status = "Actual"
                    }
                },
                new NwsAlertFeature
                {
                    Properties = new NwsAlertProperties
                    {
                        Event = "Test",
                        Status = "Exercise"
                    }
                }
            ]
        };

        var mapped = WeatherResponseMapper.MapAlerts(response);

        Assert.Single(mapped);
        var alert = mapped[0];
        Assert.Equal("Heat Advisory", alert.EventTitle);
        Assert.Equal("High temperatures expected", alert.Headline);
        Assert.Equal("Severe", alert.Severity);
        Assert.Equal("Expected", alert.Urgency);
        Assert.Equal("Likely", alert.Certainty);
        Assert.Equal(new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero), alert.EffectiveTime);
        Assert.Equal(new DateTimeOffset(2026, 1, 1, 20, 0, 0, TimeSpan.Zero), alert.ExpirationTime);
        Assert.Equal("Stay hydrated.", alert.Description);
        Assert.Equal("Drink water.", alert.Instructions);
        Assert.Equal("Jefferson County", alert.Area);
    }
}
