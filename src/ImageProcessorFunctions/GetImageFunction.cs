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

public class GetImageFunction
{
    private readonly IBlobService _blobService;

    public GetImageFunction(IBlobService blobService)
    {
        _blobService = blobService;
    }
    [FunctionName("GetImageFunction")]
    public async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req, ILogger log)
    {
        log.LogInformation("C# HTTP trigger function processed a request.");

        string image = req.Query["image"];
        if (image is null)
        {
            return new BadRequestObjectResult("Please pass a image on the query string");
        }

        try
        {
            var (blobStream, contentType) = await _blobService.GetBlobAsync(image);
            return new FileStreamResult(blobStream, contentType);
        }
        catch (Exception ex)
        {
            return new BadRequestObjectResult($"Error: {ex.Message}");
        }
        
    }
    
    private string DetermineContentType(string filename)
    {
        var fileExtension = Path.GetExtension(filename).ToLowerInvariant();
        return fileExtension switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            _ => "application/octet-stream"
        };
    }
}