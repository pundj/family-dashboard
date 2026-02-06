using FamilyDashboard.Blazor.Models;
using System.Net.Http.Json;

namespace FamilyDashboard.Blazor.Services;
public class RandomQuoteService : IRandomQuoteService
{
    private readonly HttpClient _httpClient;

    public RandomQuoteService(HttpClient httpClient) => _httpClient = httpClient;

    public async Task<GetRandomQuoteResponse> GetQuoteAsync() =>
        (await _httpClient.GetFromJsonAsync<GetRandomQuoteResponse[]>("https://bible-api.com/data/dra/random"))?.FirstOrDefault() ?? new();
}

