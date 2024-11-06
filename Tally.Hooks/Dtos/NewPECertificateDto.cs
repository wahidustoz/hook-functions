using Tally.Hooks.Entities;

namespace Tally.Hooks.Dtos;

public class NewPECertificate
{
    public string? Holder { get; set; }
    public int LearningHours { get; set; }
    public DateOnly From { get; set; }
    public DateOnly To { get; set; }

    public ProfessionalEnhancementCertificate ToEntity()
    => new()
    {
        Holder = Holder,
        LearningHours = LearningHours,
        From = From,
        To = To
    };
}

public class CertificateDto
{
    public int Id { get; set; }
    public string? Type { get; set; }
    public string? Holder { get; set; }
    public string? Number { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class PECertificateDto : CertificateDto
{
    public DateOnly From { get; set; }
    public DateOnly To { get; set; }
    public int LearningHours { get; set; }

    public static PECertificateDto FromEntity(ProfessionalEnhancementCertificate entity)
    => new()
    {
        Id = entity.Id,
        Number = entity.Number,
        Holder = entity.Holder,
        Type = entity.Type.ToString(),
        LearningHours = entity.LearningHours,
        From = entity.From,
        To = entity.To,
        CreatedAt = entity.CreatedAt
    };
}