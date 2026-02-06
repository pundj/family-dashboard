using System.Text.Json.Serialization;

namespace FamilyDashboard.Blazor.Models
{
    public class GetRandomQuoteResponse
    {
        public string? Content { get; set; }
        public string? Author { get; set; }
        public Translation? Translation { get; set; }
        public RandomVerse? RandomVerse { get; set; }
    }

    public record Translation
    {
        public string? Name { get; set; }
    }

    public record RandomVerse
    {
        public string Book { get; set; } = string.Empty;
        public string Chapter { get; set; } = string.Empty;
        public string Verse { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
    }
}
