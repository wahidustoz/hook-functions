namespace Tally.Hooks.Entities;

public class Certificate
{
    public int Id { get; set; }
    public ECertificateType Type { get; set; }
    public string? Holder { get; set; }
    public string? Number { get; set; }
    public DateOnly IssuedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}
