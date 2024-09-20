using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Tally.Hooks.Models;
using Telegram.Bot;

namespace Tally.Hooks;

public class TallyTeacherReportsHook(
    ILogger<TallyTeacherReportsHook> logger,
    ITelegramBotClient botClient,
    IConfiguration configuration,
    IWebHostEnvironment hostingEnvironment)
{
    [Function("tally-teacher-reports-hook")]
    public async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest request,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Starting to process Tally Hook for Teacher report.");

        string requestBody = await new StreamReader(request.Body).ReadToEndAsync(cancellationToken);
        logger.LogInformation("Request Body: {requestBody}", requestBody);

        TallyRequest? tallyRequest;
        try
        {
            tallyRequest = JsonSerializer.Deserialize<TallyRequest>(requestBody);
            if (tallyRequest is null)
            {
                logger.LogWarning("Deserialized TallyRequest is null.");
                return new BadRequestObjectResult("Invalid request payload.");
            }

            var bot = await botClient.GetMeAsync(cancellationToken);
            logger.LogInformation("There is a connection to {username} bot. Trying to send message to group.", bot.Username);

            var chatId = configuration.GetValue<long>("Bot:TeacherReports:GroupId");

            var message = await botClient.SendTextMessageAsync(
                chatId, 
                FormatReport(tallyRequest),
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Html,
                cancellationToken: cancellationToken);

            if (hostingEnvironment.IsDevelopment())
            {
                await Task.Delay(10000, cancellationToken);
                await botClient.DeleteMessageAsync(chatId, message.MessageId);
            }

        }
        catch (JsonException ex)
        {
            logger.LogError(ex, "Error deserializing request body.");
            return new BadRequestObjectResult("Invalid JSON format.");
        }

        return new OkObjectResult("Hook processed!");
    }

    private string FormatReport(TallyRequest request)
    {
        return $@"
<b>⭐ {request.Data?.FormName} ⭐</b>

<b>🧑‍🏫 Ustoz:</b> {GetFieldValue("Ustoz")}
<b>📖 Guruh:</b> {GetFieldValue("Guruh nomi")}
<b>🔖 Mavzu:</b> {GetFieldValue("Mavzu")}
<b>➕ Keldi:</b> {GetFieldValue("Nechta keldi")}
<b>➖ Kelmadi:</b> {GetFieldValue("Nechta kelmadi")}

<b> ✍️ Izoh:</b> {GetFieldValue("Qo'shimcha izoh")}";


        string GetFieldValue(string label) =>
            $"{request.Data!.Fields!.First(f => f.Label!.Equals(label, StringComparison.InvariantCultureIgnoreCase)).Value}";
    }
}
 