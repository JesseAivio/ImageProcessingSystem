using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ImageProcessorFunctions.Services;

public interface IBlobService
{
    public Task<(Stream Stream, string ContentType)> GetBlobAsync(string name);

    public Task UploadFileBlobAsync(string filePath, string fileName);
        
    public Task UploadContentBlobAsync(string content, string fileName);

    public Task DeleteBlobAsync(string blobName);
    
    public Task DeleteAllBlobsInContainerAsync();
}