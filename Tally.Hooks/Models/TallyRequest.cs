using System.Text.Json.Serialization;

namespace Tally.Hooks.Models;

public class TallyRequest
{
    [JsonPropertyName("eventId")]
    public string? EventId { get; set; }

    [JsonPropertyName("eventType")]
    public string? EventType { get; set; }

    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }

    [JsonPropertyName("data")]
    public TallyData? Data { get; set; }
}
