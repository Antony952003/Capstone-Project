using Microsoft.Extensions.Options;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using EmployeeGrievanceRedressal.Models.AzureConfiguration;

public class BlobStorageService
{
    private readonly AzureBlobStorageSettings _blobStorageSettings;

    public BlobStorageService(IOptions<AzureBlobStorageSettings> blobStorageSettings)
    {
        _blobStorageSettings = blobStorageSettings.Value;
    }

    public async Task<List<string>> UploadFilesAsync(IFormFile[] files)
    {
        var documentUrls = new List<string>();

        foreach (var file in files)
        {
            var url = await UploadToBlobStorage(file);
            documentUrls.Add(url);
        }

        return documentUrls;
    }

    private async Task<string> UploadToBlobStorage(IFormFile file)
    {
        var blobServiceClient = new BlobServiceClient(_blobStorageSettings.ConnectionString);
        var containerClient = blobServiceClient.GetBlobContainerClient(_blobStorageSettings.ContainerName);
        var blobClient = containerClient.GetBlobClient(file.FileName);

        using (var stream = file.OpenReadStream())
        {
            await blobClient.UploadAsync(stream, true);
        }

        return blobClient.Uri.ToString();
    }
}
