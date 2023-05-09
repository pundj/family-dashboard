using FamilyDashboard.Blazor.Models;
using System.Net.Http.Json;

namespace FamilyDashboard.Blazor.Services;

public class RandomQuoteService : IRandomQuoteService
{
    private readonly HttpClient _httpClient;

    public RandomQuoteService(IHttpClientFactory httpClientFactory) => _httpClient = httpClientFactory.CreateClient("Quote");

    public async Task<GetRandomQuoteResponse> GetQuoteAsync() =>
        (await _httpClient.GetFromJsonAsync<GetRandomQuoteResponse[]>("https://api.quotable.io/quotes/random"))?.FirstOrDefault() ?? new();
}
