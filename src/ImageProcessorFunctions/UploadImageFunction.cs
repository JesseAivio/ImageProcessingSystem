using System;
using System.IO;
using System.Threading.Tasks;
using ImageProcessorFunctions.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ImageProcessorFunctions;

public class UploadImageFunction
{
    private readonly IBlobService _blobService;

    public UploadImageFunction(IBlobService blobService)
    {
        _blobService = blobService;
    }
    
    [FunctionName("UploadImageFunction")]
    public async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req, ILogger log)
    {
        log.LogInformation("C# HTTP trigger function processed a request.");
        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        dynamic data = JsonConvert.DeserializeObject(requestBody);
        
        var filePath = data?.filePath;
        if (filePath is null)
        {
            return new BadRequestObjectResult("Please pass a filePath on the request body");
        }
        string fileName = Guid.NewGuid().ToString();
        try
        {
            await _blobService.UploadFileBlobAsync(filePath.ToString(), fileName);
        }
        catch (Exception ex)
        {
            return new BadRequestObjectResult($"Error: {ex.Message}");
        }



        return (ActionResult)new OkObjectResult($"{fileName.ToString()}");

    }
}