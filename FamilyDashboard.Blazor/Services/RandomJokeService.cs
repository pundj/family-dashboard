using FamilyDashboard.Blazor.Models;
using System.Net.Http.Json;

namespace FamilyDashboard.Blazor.Services;

public class RandomJokeService : IRandomJokeService
{
    private readonly HttpClient _httpClient;

    public RandomJokeService(HttpClient httpClient) => _httpClient = httpClient;

    public async Task<GetRandomJokeResponse> GetJokeAsync() => 
        (await _httpClient.GetFromJsonAsync<GetRandomJokeResponse>("https://official-joke-api.appspot.com/random_joke")) ?? new();
}

