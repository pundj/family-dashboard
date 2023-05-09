using FamilyDashboard.Blazor.Models;

namespace FamilyDashboard.Blazor.Services;

public interface IRandomQuoteService
{
    Task<GetRandomQuoteResponse> GetQuoteAsync();
}
