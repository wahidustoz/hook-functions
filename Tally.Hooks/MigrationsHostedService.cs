using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Tally.Hooks.Data;

namespace Tally.Hooks;

public class MigrationsHostedService(
    ILogger<MigrationsHostedService> logger,
    IConfiguration configuration,
    IServiceScopeFactory scopeFactory) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<IFunctionsDbContext>();

        if (configuration.GetValue<bool>("MigrateDatabase"))
        {
            logger.LogInformation("Migrating database...");
            await dbContext.Database.MigrateAsync(cancellationToken);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
        => Task.CompletedTask;
}