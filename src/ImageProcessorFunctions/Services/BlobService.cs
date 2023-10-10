using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace ImageProcessorFunctions.Services;

public class BlobService : IBlobService
    {
        private readonly BlobServiceClient _blobServiceClient;

        public BlobService(BlobServiceClient blobServiceClient)
        {
            _blobServiceClient = blobServiceClient;
        }

        public async Task<(Stream Stream, string ContentType)> GetBlobAsync(string name)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient("images");
            var blobClient = containerClient.GetBlobClient(name);
            if(await blobClient.ExistsAsync())
            {
                var response = await blobClient.DownloadAsync();
                return (response.Value.Content, response.Value.Details.ContentType);
            }
            else
            {
                throw new InvalidOperationException($"Blob {name} not found in container images");
            }
        }

        public async Task UploadFileBlobAsync(string filePath, string fileName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient("images");
            var blobClient = containerClient.GetBlobClient(fileName);
            var contentType = GetContentType(filePath);
            
            await blobClient.UploadAsync(filePath, new BlobHttpHeaders {ContentType = contentType});
        }

        public async Task UploadContentBlobAsync(string content, string fileName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient("images");
            var blobClient = containerClient.GetBlobClient(fileName);
            var contentType = GetContentType(fileName);
            var bytes = Encoding.UTF8.GetBytes(content);
            await using var memoryStream = new MemoryStream(bytes);
            await blobClient.UploadAsync(memoryStream, new BlobHttpHeaders {ContentType = contentType});
        }

        public async Task DeleteBlobAsync(string blobName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient("images");
            var blobClient = containerClient.GetBlobClient(blobName);
            await blobClient.DeleteIfExistsAsync();
        }

        public async Task DeleteAllBlobsInContainerAsync()
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient("images");
    
            await foreach (var blobItem in containerClient.FindBlobsByTagsAsync($"@container='images'"))
            {
                var blobClient = containerClient.GetBlobClient(blobItem.BlobName);
                await blobClient.DeleteIfExistsAsync();
            }
        }

        private string GetContentType(string path)
        {
            var fileExtension = Path.GetExtension(path).ToLowerInvariant();
            switch (fileExtension)
            {
                case ".txt":
                    return MediaTypeNames.Text.Plain;
                case ".jpg":
                    return "image/jpeg";
                case ".jpeg":
                    return "image/jpeg";
                case ".png":
                    return "image/png";
                default:
                    return MediaTypeNames.Application.Octet;
            }
        }
    }