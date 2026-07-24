using FamilyDashboard.Blazor.Models;
using System.Net.Http.Json;

namespace FamilyDashboard.Blazor.Services;
public class RandomQuoteService(HttpClient httpClient) : IRandomQuoteService
{
    public async Task<GetRandomQuoteResponse> GetQuoteAsync() =>
        await httpClient.GetFromJsonAsync<GetRandomQuoteResponse>("https://bible-api.com/data/dra/random") ?? new();
}

