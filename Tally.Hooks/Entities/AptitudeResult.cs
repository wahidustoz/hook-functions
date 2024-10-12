using System.Text.Json;

namespace Tally.Hooks.Entities;

public class AptitudeResult
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public int Grade { get; set; }
    public Dictionary<string, int> CategoryScores { get; set; } = new();
}
