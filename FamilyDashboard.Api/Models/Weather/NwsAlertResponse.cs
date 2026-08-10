using System.Text.Json.Serialization;

namespace FamilyDashboard.Api.Models.Weather;

public sealed class NwsAlertResponse
{
    [JsonPropertyName("features")]
    public NwsAlertFeature[]? Features { get; set; }
}

public sealed class NwsAlertFeature
{
    [JsonPropertyName("properties")]
    public NwsAlertProperties? Properties { get; set; }
}

public sealed class NwsAlertProperties
{
    [JsonPropertyName("event")]
    public string? Event { get; set; }

    [JsonPropertyName("headline")]
    public string? Headline { get; set; }

    [JsonPropertyName("severity")]
    public string? Severity { get; set; }

    [JsonPropertyName("urgency")]
    public string? Urgency { get; set; }

    [JsonPropertyName("certainty")]
    public string? Certainty { get; set; }

    [JsonPropertyName("effective")]
    public DateTimeOffset? Effective { get; set; }

    [JsonPropertyName("expires")]
    public DateTimeOffset? Expires { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("instruction")]
    public string? Instruction { get; set; }

    [JsonPropertyName("areaDesc")]
    public string? AreaDesc { get; set; }

    [JsonPropertyName("status")]
    public string? Status { get; set; }
}
