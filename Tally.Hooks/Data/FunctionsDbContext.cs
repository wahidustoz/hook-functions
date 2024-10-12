using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Tally.Hooks.Entities;

namespace Tally.Hooks.Data;

public class FunctionsDbContext(DbContextOptions<FunctionsDbContext> options) 
    : DbContext(options), IFunctionsDbContext
{
    public DbSet<AptitudeResult> AptitudeResults { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
}
