using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Tally.Hooks.Entities;

namespace Tally.Hooks.Data;

public interface IFunctionsDbContext
{
    DbSet<AptitudeResult> AptitudeResults { get; set; }
    DatabaseFacade Database { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
