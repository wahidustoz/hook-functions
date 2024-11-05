namespace Tally.Hooks.Entities;

public abstract class Certificate
{
    public int Id { get; set; }
    public ECertificateType Type { get; set; }
    public string? Holder { get; set; }
    public string? Number { get; set; }
    public DateTime CreatedAt { get; set; }
}
