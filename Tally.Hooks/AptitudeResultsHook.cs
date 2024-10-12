using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Tally.Hooks.Data;
using Tally.Hooks.Entities;
using Tally.Hooks.Extensions;

namespace Tally.Hooks;

public class AptitudeResultsHook(
    ILogger<AptitudeResultsHook> logger, 
    IFunctionsDbContext dbContext)
{
    [Function("aptitude-results-hook")]
    public async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest request, 
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Aptitude Results Hook triggered with POST method.");

        string requestBody = await new StreamReader(request.Body).ReadToEndAsync(cancellationToken);
        logger.LogInformation("Request Body: {requestBody}", requestBody);

        try
        {
            var jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var aptitudeResult = JsonSerializer.Deserialize<Models.AptitudeResult>(requestBody, jsonOptions);
            if (aptitudeResult == null)
            {
                logger.LogWarning("Deserialized AptitudeResult is null.");
                return new BadRequestObjectResult("Invalid request payload.");
            }

            foreach(var categoryScore in aptitudeResult.CategoryScores)
            {
                var userKey = categoryScore.Key;
                var userData = userKey.Split(':');
                var entity = new AptitudeResult() 
                { 
                    Name = string.Join(" ", userData[0].Split("-")?.Select(x => x.ToCapitalCase()) ?? []),
                    Phone = userData[1],
                    Grade = int.Parse(userData[2]),
                    CategoryScores = categoryScore.Value.ToDictionary(x => x.Key, x => x.Value),
                    Timestamp = DateTimeOffset.FromUnixTimeSeconds(int.Parse(userData[3]))
                };

                dbContext.AptitudeResults.Add(entity);
            }

            await dbContext.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Aptitude results processed successfully.");
        }
        catch (JsonException ex)
        {
            logger.LogError(ex, "Error deserializing request body.");
            return new BadRequestObjectResult("Invalid JSON format.");
        }

        return new OkObjectResult("Aptitude result processed successfully.");
    }
}
