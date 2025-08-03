using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using BulkStorageContainerUpdater.Models;

namespace BulkStorageContainerUpdater.Services
{
    public interface IAzureBlobService
    {
        Task DownloadBlobsAsync(BlobClient blobClient, string downloadPath);
        Task<BlobItem[]> GetBlobContainerItemsForDownload(BlobContainerClient containerClient);
        Task UploadToBlobStorageAsync(BlobContainerClient blobContainerClient, BlobItemForUpload[] blobsToUpload);
    }
}