namespace Tally.Hooks.Entities;

public class ProfessionalEnhancementCertificate : Certificate
{
    public DateOnly From { get; set; }
    public DateOnly To { get; set; }
    public int LearningHours { get; set; } 
}