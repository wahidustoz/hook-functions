using System.Text.Json.Serialization;

namespace Tally.Hooks.Models;

public class TallyField
{
    [JsonPropertyName("key")]
    public string? Key { get; set; }

    [JsonPropertyName("label")]
    public string? Label { get; set; }

    [JsonPropertyName("type"), JsonConverter(typeof(JsonStringEnumConverter<TallyFieldType>))]
    public TallyFieldType Type { get; set; }

    [JsonPropertyName("value")]
    public object? Value { get; set; }
}