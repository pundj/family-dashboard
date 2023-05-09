namespace FamilyDashboard.Blazor.Models
{
    public class GetRandomJokeResponse
    {
        public int Id { get; set; }
        public string? Type { get; set; }
        public string? Setup { get; set; }
        public string? Punchline { get; set; }
    }
}
