using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Polling;

namespace Tally.Hooks;

public class BotHostedService(
    ILogger<BotHostedService> logger,
    IServiceScopeFactory serviceScopeFactory) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("BotHostedService is starting.");
        
        try
        {
            using var scope = serviceScopeFactory.CreateScope();
            logger.LogDebug("Created service scope for bot initialization.");

            var botClient = scope.ServiceProvider.GetRequiredService<ITelegramBotClient>();
            logger.LogDebug("Retrieved ITelegramBotClient from service provider.");

            var updateHandler = scope.ServiceProvider.GetRequiredService<IUpdateHandler>();
            logger.LogDebug("Retrieved IUpdateHandler from service provider.");

            botClient.StartReceiving(
                updateHandler: updateHandler,
                receiverOptions: new(),
                cancellationToken: cancellationToken);

            logger.LogInformation("Bot started receiving updates successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while starting the BotHostedService.");
            throw;
        }

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
        => Task.CompletedTask;
}
