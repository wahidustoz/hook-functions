namespace Tally.Hooks.Models;

public class AptitudeResult
{
    public string Name { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public int Grade { get; set; }
    public Dictionary<string, Dictionary<string, int>> CategoryScores { get; set; } = new();
}