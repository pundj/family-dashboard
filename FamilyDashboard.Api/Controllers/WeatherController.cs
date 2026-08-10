using FamilyDashboard.Api.Services;
using FamilyDashboard.Blazor.Models.Weather;
using Microsoft.AspNetCore.Mvc;

namespace FamilyDashboard.Api.Controllers;

[ApiController]
[Route("api/weather")]
public sealed class WeatherController(IWeatherProxyService weatherProxyService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<WeatherData>> Get([FromQuery] double latitude, [FromQuery] double longitude, [FromQuery] string? locationName, CancellationToken cancellationToken)
    {
        if (double.IsNaN(latitude) || double.IsNaN(longitude) || double.IsInfinity(latitude) || double.IsInfinity(longitude))
        {
            return BadRequest("Latitude and longitude are required.");
        }

        var weather = await weatherProxyService.GetWeatherAsync(latitude, longitude, locationName, cancellationToken);
        return Ok(weather);
    }
}
