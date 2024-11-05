using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Tally.Hooks;
using Tally.Hooks.Data;
using Telegram.Bot;
using Telegram.Bot.Polling;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureAppConfiguration((context, config) =>
    {
        config.AddJsonFile("appsettings.json", optional: true);
        if(context.HostingEnvironment.IsDevelopment())
            config.AddJsonFile("appsettings.Development.json", optional: true);

        if(IsAppConfigEnabled() && Environment.GetEnvironmentVariable("AppConfig__ConnectionString") is string connectionString)
        {
            config.AddAzureAppConfiguration(o =>
            {
                o.Connect(connectionString);
                o.Select(KeyFilter.Any, LabelFilter.Null);

                var labels = Environment.GetEnvironmentVariable("AppConfig__Labels")?.Split(',') ?? [];
                foreach (var label in labels)
                    o.Select(KeyFilter.Any, label);
            });
        }
    })
    .ConfigureServices((context, services) =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        services.AddTransient<ITelegramBotClient, TelegramBotClient>(provider =>
        {
            var token = provider.GetRequiredService<IConfiguration>()["Bot:Token"];
            return new TelegramBotClient(token ?? throw new Exception("Bot token not configured."));
        });

        var connectionString = context.Configuration.GetConnectionString("FunctionsConnection") 
            ?? throw new Exception("Functions connection string not configured.");
        services.AddDbContext<IFunctionsDbContext, FunctionsDbContext>(options => options.UseNpgsql(connectionString));

        services.AddScoped<IUpdateHandler, BotUpdateHandler>();
        services.AddHostedService<MigrationsHostedService>();
        services.AddHostedService<BotHostedService>();
    })
    .ConfigureLogging(logging =>
    {
        logging.Services.Configure<LoggerFilterOptions>(options =>
        {
            LoggerFilterRule? defaultRule = options.Rules.FirstOrDefault(rule => rule.ProviderName
                is "Microsoft.Extensions.Logging.ApplicationInsights.ApplicationInsightsLoggerProvider");
            if (defaultRule is not null)
            {
                options.Rules.Remove(defaultRule);
            }
        });
    })
    .Build();

host.Run();

static bool IsAppConfigEnabled() => 
    Environment.GetEnvironmentVariable("AppConfig__Enabled") is string enabled && 
    string.Equals(enabled, "true", StringComparison.OrdinalIgnoreCase);
