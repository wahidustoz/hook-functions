using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Tally.Hooks.Data;
using Telegram.Bot;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureAppConfiguration(config =>
    {
        config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
              .AddEnvironmentVariables();
    })
    .ConfigureServices((context, services) =>
    {
        // Add Application Insights telemetry
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        // Configure Telegram Bot Client
        services.AddTransient<ITelegramBotClient, TelegramBotClient>(provider =>
        {
            var token = provider.GetRequiredService<IConfiguration>()["Bot:Token"];
            return new TelegramBotClient(token ?? throw new Exception("Bot token not configured."));
        });

        // Register AppDbContext with PostgreSQL connection
        var connectionString = context.Configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString));

        // Add other necessary services if needed
    })
    .Build();

host.Run();
