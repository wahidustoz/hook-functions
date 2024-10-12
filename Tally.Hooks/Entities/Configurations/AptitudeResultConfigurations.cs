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
        builder.Property(e => e.Name).HasMaxLength(100);
        builder.Property(e => e.Phone).HasMaxLength(15);
        builder.Property(e => e.CategoryScores)
                .HasColumnType("jsonb")
                .HasConversion(
                    v => JsonSerializer.Serialize(v, JsonSerializerOptions.Default),
                    v => JsonSerializer.Deserialize<Dictionary<string, int>>(v, JsonSerializerOptions.Default) ?? new());
    }
}