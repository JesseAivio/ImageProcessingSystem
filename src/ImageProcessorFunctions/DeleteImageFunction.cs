using System;
using System.IO;
using System.Threading.Tasks;
using ImageProcessorFunctions.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ImageProcessorFunctions;

public class DeleteImageFunction
{
    private readonly IBlobService _blobService;

    public DeleteImageFunction(IBlobService blobService)
    {
        _blobService = blobService;
    }
    [FunctionName("DeleteImageFunction")]
    public async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Function,  "delete", Route = null)] HttpRequest req, ILogger log)
    {
        log.LogInformation("C# HTTP trigger function processed a request.");

        string image = req.Query["image"];
        if (image is null)
        {
            return new BadRequestObjectResult("Please pass a image on the query string");
        }

        try
        {
            await _blobService.DeleteBlobAsync(image);
        }
        catch (Exception ex)
        {
            return new BadRequestObjectResult($"Error: {ex.Message}");
        }

        return (ActionResult)new OkObjectResult($"{image} removed!");
        
    }
}