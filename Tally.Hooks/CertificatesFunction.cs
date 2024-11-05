using System.Text.Json;
using Grpc.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Tally.Hooks.Data;
using Tally.Hooks.Dtos;

namespace Tally.Hooks;

public class CertificatesFunction(
    IFunctionsDbContext context,
    ILogger<CertificatesFunction> logger)
{
    readonly JsonSerializerOptions jsonOptions = new() { PropertyNameCaseInsensitive = true };

    [Function("create-pe-certificate")]
    public async Task<IActionResult> CreateProfessionalEnhancement(
        [HttpTrigger(
            AuthorizationLevel.Function, 
            "post",
            Route = "pe-certificates")] 
        HttpRequest request)
    {
        try
        {
            var dto = await JsonSerializer.DeserializeAsync<NewPECertificate>(request.Body, jsonOptions)
                ?? throw new Exception($"Deserialization to {typeof(NewPECertificate).Name} failed for {request.Body}");
            var entry = context.ProfessionalEnhancementCertificates.Add(dto.ToEntity());
            await context.SaveChangesAsync();

            return new OkObjectResult(PECertificateDto.FromEntity(entry.Entity));
        }
        catch(Exception ex)
        {
            logger.LogError(ex, "Error while creating a Professional Enhancement certificate.");
            return new StatusCodeResult(500);
        }
    }

    [Function("get-pe-certificate")]
    public async Task<IActionResult> ReadProfessionalEnhancement(
        [HttpTrigger(
            AuthorizationLevel.Anonymous, 
            "get", 
            Route = "pe-certificates/{number}")] 
        HttpRequest request,
        string number)
    {
        if (string.IsNullOrWhiteSpace(number))
            return new StatusCodeResult(400);
        
        var entity = await context.ProfessionalEnhancementCertificates
            .FirstOrDefaultAsync(x => x.Number == number);
        
        if(entity is null)
            return new StatusCodeResult(404);
        
        return new OkObjectResult(PECertificateDto.FromEntity(entity));
    }

    [Function("get-pe-certificates")]
    public async Task<IActionResult> ReadProfessionalEnhancements(
        [HttpTrigger(
            AuthorizationLevel.Anonymous, 
            "get", 
            Route = "pe-certificates")] 
        HttpRequest request,
        string number)
        => new OkObjectResult(await context.ProfessionalEnhancementCertificates
                .Select(x => PECertificateDto.FromEntity(x))
                .ToArrayAsync());
} 