using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Tally.Hooks.ResultModels;

namespace Tally.Hooks.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<AptitudeResult> AptitudeResults { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<AptitudeResult>()
                .Property(e => e.CategoryScores)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, new JsonSerializerOptions { WriteIndented = true }),
                    v => JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, int>>>(v, new JsonSerializerOptions()) ?? new Dictionary<string, Dictionary<string, int>>()) // Handles nulls
                .Metadata.SetValueComparer(new ValueComparer<Dictionary<string, Dictionary<string, int>>>( 
                    (c1, c2) => c1 != null && c2 != null && c1.SequenceEqual(c2), // Ensure neither is null
                    c => c == null ? 0 : c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())), 
                    c => c == null ? new Dictionary<string, Dictionary<string, int>>() : c.ToDictionary(entry => entry.Key, entry => entry.Value) // Clone logic
                ));
        }
    }
}
