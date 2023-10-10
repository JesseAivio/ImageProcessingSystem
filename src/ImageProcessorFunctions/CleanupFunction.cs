using System;
using System.Threading.Tasks;
using ImageProcessorFunctions.Services;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace ImageProcessorFunctions;

public class CleanupFunction
{
    private readonly IBlobService _blobService;

    public CleanupFunction(IBlobService blobService)
    {
        _blobService = blobService;
    }
    [FunctionName("CleanupFunction")]
    public async Task RunAsync([TimerTrigger("0 0 0 * * *")] TimerInfo myTimer, ILogger log)
    {
        log.LogInformation($"C# Timer trigger function executed at: {DateTime.UtcNow}");
        await _blobService.DeleteAllBlobsInContainerAsync();
        log.LogInformation($"removed all blobs at: {DateTime.UtcNow}");
    }
}