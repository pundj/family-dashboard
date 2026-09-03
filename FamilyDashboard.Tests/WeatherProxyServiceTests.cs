using System.Reflection;
using FamilyDashboard.Api.Services;

namespace FamilyDashboard.Tests;

public class WeatherProxyServiceTests
{
    [Fact]
    public void BuildForecastRequestUri_UsesHgefsEnsembleMeanModel()
    {
        var method = typeof(WeatherProxyService).GetMethod("BuildForecastRequestUri", BindingFlags.NonPublic | BindingFlags.Static);

        Assert.NotNull(method);

        var requestUri = method!.Invoke(null, [39.742, -84.388]) as string
            ?? throw new InvalidOperationException("Expected the weather forecast URI builder to return a request string.");

        Assert.True(requestUri.Contains("latitude=39.742", StringComparison.Ordinal));
        Assert.True(requestUri.Contains("longitude=-84.388", StringComparison.Ordinal));
        Assert.True(requestUri.Contains("models=ncep_hgefs025_ensemble_mean", StringComparison.Ordinal));
    }
}
