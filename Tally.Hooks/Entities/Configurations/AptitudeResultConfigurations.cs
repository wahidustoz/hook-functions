using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Tally.Hooks.Entities.Configurations;

public class AptitudeResultConfigurations : IEntityTypeConfiguration<AptitudeResult>
{
    public void Configure(EntityTypeBuilder<AptitudeResult> builder) 
    {
        builder.HasKey(e => e.Id);
        builder.HasIndex(e => e.Timestamp);
        builder.Property(e => e.Name).HasMaxLength(100);
        builder.Property(e => e.Phone).HasMaxLength(15);
        builder.Property(e => e.CategoryScores)
                .HasColumnType("jsonb")
                .HasConversion(
                    v => JsonSerializer.Serialize(v, JsonSerializerOptions.Default),
                    v => JsonSerializer.Deserialize<Dictionary<string, int>>(v, JsonSerializerOptions.Default) ?? new());
    }
}

public class CertificateTypeConfigurations : IEntityTypeConfiguration<Certificate>
{
    public void Configure(EntityTypeBuilder<Certificate> builder)
    {
        builder.HasDiscriminator(x => x.Type)
            .HasValue<ProfessionalEnhancementCertificate>(
                ECertificateType.ProfessionalEnhancement);
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.Number)
            .HasMaxLength(5)
            .HasComputedColumnSql("substring(md5(\"Id\"::text) from 0 for 6)", stored: true);
        builder.HasIndex(x => x.Number);
    }
}