using System;
using Azure.Identity;
using Azure.Storage.Blobs;
using ImageProcessorFunctions.Services;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(ImageProcessorFunctions.Startup))]
namespace ImageProcessorFunctions;

public class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        var isDevelopment = string.IsNullOrEmpty(Environment.GetEnvironmentVariable("WEBSITE_INSTANCE_ID"));
        
        var configBuilder = new ConfigurationBuilder()
            .SetBasePath(Environment.CurrentDirectory)
            .AddEnvironmentVariables();
        
        if (isDevelopment)
        {
            configBuilder.AddUserSecrets<Startup>();
        }
        else
        {
            var builtConfig = configBuilder.Build();
            var keyVaultUrl = builtConfig["KeyvaultUri"]; 

            var tokenCredential = new DefaultAzureCredential(); 
            configBuilder.AddAzureKeyVault(new Uri(keyVaultUrl), tokenCredential);
        }
        
        var config = configBuilder.Build();


        builder.Services.AddSingleton<IConfiguration>(config);

        builder.Services.AddSingleton<IConfiguration>(config);

        builder.Services.AddSingleton(x => 
            new BlobServiceClient(config["StorageAccountConnectionString"]));
        builder.Services.AddSingleton<IBlobService, BlobService>();
    }
}