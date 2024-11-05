using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Tally.Hooks.Entities.Configurations;

public class CertificateTypeConfigurations : IEntityTypeConfiguration<Certificate>
{
    public void Configure(EntityTypeBuilder<Certificate> builder)
    {
        builder.ToTable("Certificates")
            .HasDiscriminator(x => x.Type)
            .HasValue<ProfessionalEnhancementCertificate>(
                ECertificateType.ProfessionalEnhancement);
                
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.Number)
            .HasMaxLength(5)
            .HasComputedColumnSql("substring(md5(\"Id\"::text) from 0 for 6)", stored: true);
        builder.HasIndex(x => x.Number);
        builder.Property(x => x.Holder)
            .HasMaxLength(255)
            .IsRequired();
        builder.Property(x => x.CreatedAt)
            .HasDefaultValueSql("now()");
    }
}