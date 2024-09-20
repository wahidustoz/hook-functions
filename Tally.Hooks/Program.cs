using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        services.AddTransient<ITelegramBotClient, TelegramBotClient>(provider =>
        {
            var token = provider.GetRequiredService<IConfiguration>()["Bot:Token"];
            return new TelegramBotClient(token ?? throw new Exception("Bot token not configured."));
        });
    })
    .Build();

host.Run();
