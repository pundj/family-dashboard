using FamilyDashboard.Api.Data;
using FamilyDashboard.Api.Entities;
using FamilyDashboard.Blazor.Models.Calendar;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace FamilyDashboard.Api.Controllers;

[ApiController]
[Route("api/calendar/preferences")]
public class CalendarPreferencesController(ApplicationDbContext dbContext) : ControllerBase
{
    private const string PreferenceKey = "default";
    private static readonly JsonSerializerOptions JsonSerializerOptions = new(JsonSerializerDefaults.Web);

    [HttpGet]
    public async Task<ActionResult<CalendarPreferences>> Get()
    {
        var entity = await dbContext.CalendarPreferences
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.PreferenceKey == PreferenceKey);

        if (entity is null || string.IsNullOrWhiteSpace(entity.PreferencesJson))
            return Ok(new CalendarPreferences());

        try
        {
            var prefs = JsonSerializer.Deserialize<CalendarPreferences>(entity.PreferencesJson, JsonSerializerOptions)
                ?? new CalendarPreferences();

            return Ok(prefs);
        }
        catch
        {
            return Ok(new CalendarPreferences());
        }
    }

    [HttpPut]
    public async Task<IActionResult> Put([FromBody] CalendarPreferences preferences)
    {
        foreach (var entry in preferences.Calendars)
        {
            var calendarId = entry.Key;
            var settings = entry.Value;
            settings.CalendarId = string.IsNullOrWhiteSpace(settings.CalendarId) ? calendarId : settings.CalendarId;
            settings.Color = string.IsNullOrWhiteSpace(settings.Color) ? "#3788d8" : settings.Color;
        }

        var json = JsonSerializer.Serialize(preferences, JsonSerializerOptions);

        var entity = await dbContext.CalendarPreferences
            .SingleOrDefaultAsync(x => x.PreferenceKey == PreferenceKey);

        if (entity is null)
        {
            entity = new CalendarPreferencesEntity
            {
                PreferenceKey = PreferenceKey,
                PreferencesJson = json,
                UpdatedUtc = DateTimeOffset.UtcNow
            };
            dbContext.CalendarPreferences.Add(entity);
        }
        else
        {
            entity.PreferencesJson = json;
            entity.UpdatedUtc = DateTimeOffset.UtcNow;
        }

        await dbContext.SaveChangesAsync();
        return NoContent();
    }
}
