using FamilyDashboard.Blazor.Models.Calendar;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;
using System.Text.Json;
using System.Net.Http.Headers;

namespace FamilyDashboard.Blazor.Services;

public class GoogleCalendarService : ICalendarService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly IGoogleAuthService _authService;
    private readonly List<string> _calendarIds;

    public GoogleCalendarService(
        IHttpClientFactory httpClientFactory, 
        IConfiguration configuration,
        IGoogleAuthService authService)
    {
        _httpClient = httpClientFactory.CreateClient();
        _configuration = configuration;
        _authService = authService;
        
        var settings = _configuration.GetSection("GoogleOAuth").Get<GoogleOAuthSettings>();
        _calendarIds = settings?.CalendarIds ?? new List<string>();
    }

    public async Task<CalendarEvent?> GetNextEventAsync()
    {
        var accessToken = await _authService.GetAccessTokenAsync();

        if (string.IsNullOrEmpty(accessToken) || !_calendarIds.Any())
        {
            return null;
        }

        var now = DateTime.UtcNow;
        var end = now.AddDays(7);

        CalendarEvent? nextEvent = null;

        foreach (var calendarId in _calendarIds)
        {
            try
            {
                var candidate = await FetchNextEventForCalendarAsync(calendarId, now, end, accessToken);
                if (candidate != null && (nextEvent == null || candidate.StartTime < nextEvent.StartTime))
                {
                    nextEvent = candidate;
                }
            }
catch (Exception ex)
{
    Console.WriteLine($"Error fetching next event for calendar {calendarId}: {ex}");
}
        }

        return nextEvent;
    }

    private async Task<CalendarEvent?> FetchNextEventForCalendarAsync(
        string calendarId,
        DateTime startDate,
        DateTime endDate,
        string accessToken)
    {
        var timeMin = startDate.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
        var timeMax = endDate.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");

        var url = $"https://www.googleapis.com/calendar/v3/calendars/{Uri.EscapeDataString(calendarId)}/events" +
                  $"?timeMin={timeMin}" +
                  $"&timeMax={timeMax}" +
                  $"&singleEvents=true" +
                  $"&orderBy=startTime" +
                  $"&maxResults=1";

using var request = new HttpRequestMessage(HttpMethod.Get, url);
request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

using var response = await _httpClient.SendAsync(request);

if (!response.IsSuccessStatusCode)
{
    return null;
}

var content = await response.Content.ReadAsStringAsync();
using var jsonDoc = JsonDocument.Parse(content);
        if (jsonDoc.RootElement.TryGetProperty("items", out var items))
        {
            var enumerator = items.EnumerateArray();
            if (enumerator.MoveNext())
            {
                return ParseEvent(enumerator.Current, calendarId);
            }
        }

        return null;
    }

    public async Task<List<CalendarEvent>> GetEventsAsync(DateTime startDate, DateTime endDate)
    {
        var accessToken = await _authService.GetAccessTokenAsync();
        
        if (string.IsNullOrEmpty(accessToken) || !_calendarIds.Any())
        {
            return new List<CalendarEvent>();
        }

        var allEvents = new List<CalendarEvent>();

        foreach (var calendarId in _calendarIds)
        {
            try
            {
                var events = await FetchEventsForCalendarAsync(calendarId, startDate, endDate, accessToken);
                allEvents.AddRange(events);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching events for calendar {calendarId}: {ex.Message}");
            }
        }

        return allEvents.OrderBy(e => e.StartTime).ToList();
    }

    private async Task<List<CalendarEvent>> FetchEventsForCalendarAsync(
        string calendarId, 
        DateTime startDate, 
        DateTime endDate, 
        string accessToken)
    {
        var timeMin = startDate.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
        var timeMax = endDate.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");

        var url = $"https://www.googleapis.com/calendar/v3/calendars/{Uri.EscapeDataString(calendarId)}/events" +
                  $"?timeMin={timeMin}" +
                  $"&timeMax={timeMax}" +
                  $"&singleEvents=true" +
                  $"&orderBy=startTime";

        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Error: {response.StatusCode} - {error}");
            return new List<CalendarEvent>();
        }

        var content = await response.Content.ReadAsStringAsync();
        var jsonDoc = JsonDocument.Parse(content);
        var events = new List<CalendarEvent>();

        if (jsonDoc.RootElement.TryGetProperty("items", out var items))
        {
            foreach (var item in items.EnumerateArray())
            {
                var calEvent = ParseEvent(item, calendarId);
                if (calEvent != null)
                {
                    events.Add(calEvent);
                }
            }
        }

        return events;
    }

    private CalendarEvent? ParseEvent(JsonElement eventElement, string calendarId)
    {
        try
        {
            var id = eventElement.GetProperty("id").GetString() ?? string.Empty;
            var summary = eventElement.TryGetProperty("summary", out var summaryProp) 
                ? summaryProp.GetString() ?? "No Title" 
                : "No Title";
            
            // Clean the title by removing status indicators
            summary = CleanEventTitle(summary);
            
            var description = eventElement.TryGetProperty("description", out var descProp) 
                ? descProp.GetString() 
                : null;
            
            var location = eventElement.TryGetProperty("location", out var locProp) 
                ? locProp.GetString() 
                : null;

            var start = eventElement.GetProperty("start");
            var end = eventElement.GetProperty("end");

            DateTime startTime;
            DateTime endTime;
            bool isAllDay = false;

            if (start.TryGetProperty("dateTime", out var startDateTimeProp))
            {
                startTime = DateTime.Parse(startDateTimeProp.GetString()!);
            }
            else if (start.TryGetProperty("date", out var startDateProp))
            {
                startTime = DateTime.Parse(startDateProp.GetString()!);
                isAllDay = true;
            }
            else
            {
                return null;
            }

            if (end.TryGetProperty("dateTime", out var endDateTimeProp))
            {
                endTime = DateTime.Parse(endDateTimeProp.GetString()!);
            }
            else if (end.TryGetProperty("date", out var endDateProp))
            {
                endTime = DateTime.Parse(endDateProp.GetString()!);
            }
            else
            {
                endTime = startTime.AddHours(1);
            }

            return new CalendarEvent
            {
                Id = id,
                Title = summary,
                Description = description,
                StartTime = startTime,
                EndTime = endTime,
                Location = location,
                IsAllDay = isAllDay,
                CalendarId = calendarId
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error parsing event: {ex.Message}");
            return null;
        }
    }

    private string CleanEventTitle(string title)
    {
        if (string.IsNullOrEmpty(title))
            return title;

        // Remove leading status indicators from Google Calendar invitations
        // "?" = Maybe/Tentative
        // "?" = Accepted
        // "?" = Declined
        var cleanTitle = title.TrimStart('?', '?', '?', ' ');
        
        return string.IsNullOrWhiteSpace(cleanTitle) ? title : cleanTitle;
    }
}
