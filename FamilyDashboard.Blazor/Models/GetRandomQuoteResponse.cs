using System.Text.Json.Serialization;

namespace FamilyDashboard.Blazor.Models
{
    public class GetRandomQuoteResponse
    {
        [JsonPropertyName("content")]
        public string? Content { get; set; }

        [JsonPropertyName("author")]
        public string? Author { get; set; }

        [JsonPropertyName("translation")]
        public Translation? Translation { get; set; }

        [JsonPropertyName("random_verse")]
        public RandomVerse? RandomVerse { get; set; }
    }

    public record Translation
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }
    }

    public record RandomVerse
    {
        [JsonPropertyName("book")]
        public string Book { get; set; } = string.Empty;

        [JsonPropertyName("chapter")]
        public int Chapter { get; set; }

        [JsonPropertyName("verse")]
        public int Verse { get; set; }

        [JsonPropertyName("text")]
        public string Text { get; set; } = string.Empty;
    }
}
