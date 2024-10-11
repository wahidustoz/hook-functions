using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Tally.Hooks.ResultModels;

public class AptitudeResult
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(15)]
    [JsonPropertyName("phone")]
    public string Phone { get; set; } = string.Empty;

    [Required]
    [JsonPropertyName("grade")]
    public int Grade { get; set; }

    [Column(TypeName = "jsonb")]
    [JsonPropertyName("categoryScores")]
    public Dictionary<string, Dictionary<string, int>> CategoryScores { get; set; } = new();
}
