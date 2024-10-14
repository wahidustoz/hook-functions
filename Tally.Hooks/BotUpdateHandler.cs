using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using Tally.Hooks.Data;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Tally.Hooks;

public class BotUpdateHandler(
    ILogger<BotUpdateHandler> logger,
    IConfiguration configuration,
    IServiceScopeFactory scopeFactory) : IUpdateHandler
{
    private IFunctionsDbContext dbContext = default!;
    public Task HandlePollingErrorAsync(
        ITelegramBotClient botClient, 
        Exception exception, 
        CancellationToken cancellationToken) =>
        Task.Run(() =>
        {
            var errorMessage = exception switch
            {
                ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            logger.LogError("HandlePollingErrorAsync: {ErrorMessage}", errorMessage);
        }, cancellationToken);

    public async Task HandleUpdateAsync(
        ITelegramBotClient botClient,
        Update update,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Received update type: {UpdateType}", update.Type);

        using var scope = scopeFactory.CreateScope();
        dbContext = scope.ServiceProvider.GetRequiredService<IFunctionsDbContext>();

        if (update.Type is UpdateType.Message 
        && update?.Message?.Type is MessageType.Text)
            await HandleTextMessageAsync(botClient, update.Message, cancellationToken);
        else
            logger.LogWarning("Unhandled update type: {UpdateType}", update?.Type);
    }

    private async Task HandleTextMessageAsync(
        ITelegramBotClient botClient,
        Message message,
        CancellationToken cancellationToken)
    {
        var chatId = message.Chat.Id;
        var username = message.Chat.Username;
        var messageText = message.Text;

        logger.LogInformation("Received message from {Username} (ChatId: {ChatId}): {MessageText}", username, chatId, messageText);

        if (messageText?.Equals(BotCommands.AptitudeResults, StringComparison.OrdinalIgnoreCase) is true)
        {
            var aptitudeAdmins = configuration.GetValue("Bot:AptitudeAdmins", string.Empty)?
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .ToArray();
            
            if (aptitudeAdmins is { Length: > 0 } 
            && aptitudeAdmins.Any(x => x.Equals(username, StringComparison.OrdinalIgnoreCase)))
            {
                logger.LogInformation("Authorized user {Username} requested aptitude results", username);
                await SendAptitudeResultsAsync(botClient, chatId, cancellationToken);
            }
            else 
            {
                logger.LogWarning("Unauthorized access attempt to aptitude results by {Username}", username);
                await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: "You are not authorized to use this command, sorry.",
                    cancellationToken: cancellationToken);
            }
        }
        else
            logger.LogInformation("Unrecognized command: {MessageText}", messageText);
    }

    private async Task SendAptitudeResultsAsync(
        ITelegramBotClient botClient, 
        long chatId, 
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Fetching aptitude results from database");
        var results = await dbContext.AptitudeResults
            .OrderByDescending(r => r.Timestamp)
            .ToListAsync(cancellationToken);
        
        logger.LogInformation("Retrieved {ResultCount} aptitude results", results.Count);

        var filePath = Path.Combine(Path.GetTempPath(), "AptitudeResults.xlsx");
        logger.LogInformation("Generating Excel file at {FilePath}", filePath);

        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        using (var package = new ExcelPackage())
        {
            var worksheet = package.Workbook.Worksheets.Add("Results");
            worksheet.Cells[1, 1].Value = "Name";
            worksheet.Cells[1, 2].Value = "Phone";
            worksheet.Cells[1, 3].Value = "Grade";
            worksheet.Cells[1, 4].Value = "Category Scores";
            worksheet.Cells[1, 5].Value = "Timestamp (Tashkent Time)";

            for (int i = 0; i < results.Count; i++)
            {
                var result = results[i];
                worksheet.Cells[i + 2, 1].Value = result.Name;
                worksheet.Cells[i + 2, 2].Value = result.Phone;
                worksheet.Cells[i + 2, 3].Value = result.Grade;
                worksheet.Cells[i + 2, 4].Value = string.Join(", ", result.CategoryScores.Select(cs => $"{cs.Key}: {cs.Value}"));
                worksheet.Cells[i + 2, 5].Value = TimeZoneInfo
                    .ConvertTimeFromUtc(result.Timestamp.UtcDateTime, TimeZoneInfo.FindSystemTimeZoneById("Central Asia Standard Time"))
                    .ToString("MM/dd HH:mm");
            }
            await package.SaveAsAsync(filePath, cancellationToken);
        }

        logger.LogInformation("Excel file generated successfully");

        using var stream = new FileStream(filePath, FileMode.Open);
        logger.LogInformation("Sending aptitude results file to chat {ChatId}", chatId);
        await botClient.SendDocumentAsync(chatId, new InputFileStream(stream, "AptitudeResults.xlsx"), cancellationToken: cancellationToken);
        logger.LogInformation("Aptitude results file sent successfully to chat {ChatId}", chatId);
    }
}

public static class BotCommands
{
    public const string AptitudeResults = "/aptitude-results";
}
