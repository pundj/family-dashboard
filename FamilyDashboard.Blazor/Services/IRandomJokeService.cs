using FamilyDashboard.Blazor.Models;

namespace FamilyDashboard.Blazor.Services;

public interface IRandomJokeService
{
    Task<GetRandomJokeResponse> GetJokeAsync();
}
