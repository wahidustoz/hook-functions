using System.Text.Json.Serialization;

namespace Tally.Hooks.Models;

public class TallyData
{
    [JsonPropertyName("responseId")]
    public string? ResponseId { get; set; }

    [JsonPropertyName("submissionId")]
    public string? SubmissionId { get; set; }

    [JsonPropertyName("respondentId")]
    public string? RespondentId { get; set; }

    [JsonPropertyName("formId")]
    public string? FormId { get; set; }

    [JsonPropertyName("formName")]
    public string? FormName { get; set; }

    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }

    [JsonPropertyName("fields")]
    public List<TallyField>? Fields { get; set; }
}
