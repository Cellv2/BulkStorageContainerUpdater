using Azure;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using BulkStorageContainerUpdater.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkStorageContainerUpdater.Services;

public sealed class AzureBlobService
{
    public async Task UploadToBlobStorageAsync(BlobContainerClient blobContainerClient, BlobItemForUpload[] blobsToUpload)
    {
        foreach (var blobItem in blobsToUpload)
        {
            var shouldCreateSubDirs = blobItem.CreateSubDirWithSameName;
            if (shouldCreateSubDirs)
            {
                var childGuid = Guid.NewGuid().ToString();
                var childName = string.Join("/", [blobItem.Name, childGuid]);

                var childContent = new BinaryData("content");

                await blobContainerClient.UploadBlobAsync(childName, childContent);
            }

            await blobContainerClient.UploadBlobAsync(blobItem.Name, blobItem.Content);
        }
    }

    public async Task<BlobItem[]> GetBlobContainerItemsForDownload(BlobContainerClient containerClient)
    {
        List<BlobItem> blobItems = [];

        var containerSegments = containerClient.GetBlobsAsync().AsPages(default, 1000);

        await foreach (Page<BlobItem> blobPage in containerSegments)
        {
            foreach (BlobItem blobItem in blobPage.Values)
            {
                var isSubDirChild = blobItem.Name.Split('/').Length > 2;
                if (isSubDirChild)
                {
                    //Console.WriteLine("Blob name looks to be a subdir, skipping : {0}", blobItem.Name);
                    continue;
                }

                //Console.WriteLine("Blob name: {0}", blobItem.Name);

                blobItems.Add(blobItem);
            }
        }

        return blobItems.ToArray();
    }

    public async Task DownloadBlobsAsync(BlobClient blobClient, string downloadPath)
    {
        FileStream fileStream = File.OpenWrite(downloadPath);

        var transferOptions = new StorageTransferOptions
        {
            // Set the maximum number of parallel transfer workers
            MaximumConcurrency = 10,

            // Set the initial transfer length to 8 MiB
            InitialTransferSize = 8 * 1024 * 1024,

            // Set the maximum length of a transfer to 4 MiB
            MaximumTransferSize = 4 * 1024 * 1024
        };

        var downloadOptions = new BlobDownloadToOptions()
        {
            TransferOptions = transferOptions
        };

        await blobClient.DownloadToAsync(fileStream, downloadOptions);

        fileStream.Close();
    }
}
