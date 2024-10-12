using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;

namespace Tally.Hooks;

public class HealthFunction
{
    [Function("health")]
    public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "head")] HttpRequest request) 
        => new OkObjectResult("All is well 😀");
} 