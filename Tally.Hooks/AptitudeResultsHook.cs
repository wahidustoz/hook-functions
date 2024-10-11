using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Tally.Hooks.Data;
using Tally.Hooks.ResultModels;

namespace Tally.Hooks;

public class AptitudeResultsHook(ILogger<AptitudeResultsHook> logger, ApplicationDbContext dbContext)
{
    [Function("aptitude-results-hook")]
    public async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest request, 
            CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Aptitude Results Hook triggered with POST method.");

        string requestBody = await new StreamReader(request.Body).ReadToEndAsync(cancellationToken);
        logger.LogInformation("Request Body: {requestBody}", requestBody);

        logger.LogInformation("Request Body: {requestBody}", requestBody);

        AptitudeResult? aptitudeResult;

        try
        {
            // Deserialize the incoming JSON to the AptitudeResult model
            aptitudeResult = JsonSerializer.Deserialize<AptitudeResult>(requestBody);
            if (aptitudeResult == null)
            {
                logger.LogWarning("Deserialized AptitudeResult is null.");
                return new BadRequestObjectResult("Invalid request payload.");
            }

            await dbContext.AptitudeResults.AddAsync(aptitudeResult);
            await dbContext.SaveChangesAsync();

            logger.LogInformation("Aptitude result received: Name={Name}, Phone={Phone}, Grade={Grade}",
                aptitudeResult.Name, aptitudeResult.Phone, aptitudeResult.Grade);
        }
        catch (JsonException ex)
        {
            logger.LogError(ex, "Error deserializing request body.");
            return new BadRequestObjectResult("Invalid JSON format.");
        }

        return new OkObjectResult("Aptitude result processed successfully.");
    }
}
