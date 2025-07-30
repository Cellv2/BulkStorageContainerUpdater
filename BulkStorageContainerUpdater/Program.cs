using Azure;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

var connectionString = "";

var targetContainerName = "downloadtest";

var downloadDir = "D:\\dev\\blobTest";

BlobItemForUpload[] CreateBlobItemsForUpload(int numToCreate)
{
    var rnd = new Random();

    List<BlobItemForUpload> blobItems = new();


    for (int i = 0; i < numToCreate; i++)
    {
        var parentDirName = rnd.Next(0, 200);

        var guid = Guid.NewGuid().ToString();
        var name = string.Join("/", [parentDirName, guid]);

        var stringContent = $"id:{guid}";
        var data = new BinaryData(stringContent);

        var item = new BlobItemForUpload(
            Name: name,
            Content: data,
            CreateSubDirWithSameName: (rnd.NextDouble() >= 0.5)
        );

        blobItems.Add(item);
    }

    return blobItems.ToArray();
}

async Task UploadToBlobStorageAsync(BlobContainerClient blobContainerClient, BlobItemForUpload[] blobsToUpload)
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

async Task<BlobItem[]> GetBlobContainerItemsForDownload(BlobContainerClient containerClient)
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

async Task DownloadBlobsAsync(BlobClient blobClient, string downloadPath)
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

bool DoesDirectoryExist(string path)
{
    if (!Directory.Exists(path))
    {
        return false;
    }

    return true;
}

void CreateBackup(string directory)
{
    try
    {
        var zipPath = $"{directory}.zip";
        ZipFile.CreateFromDirectory(directory, zipPath);
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex);
    }
}

void UpdateItemUrlsContentInDirectory(string directory, string matcher, string replacement)
{
    if (!DoesDirectoryExist(directory))
    {
        Console.WriteLine($"{directory} does not exist");
        return;
    }

    var directoryItems = Directory.GetDirectories(directory);
    foreach (var item in directoryItems)
    {
        var itemContents = File.ReadAllText(item);
        var newContent = itemContents.Replace(matcher, replacement);
        File.WriteAllText(item, newContent);
    }
}

//var blobServiceClient = new BlobServiceClient(connectionString);

//var blobContainers = blobServiceClient.GetBlobContainersAsync();


var containerClient = new BlobContainerClient(connectionString, targetContainerName);
var containerBlobs = containerClient.GetBlobsAsync();

//var blobsToUpload = CreateBlobItemsForUpload(50);
//await UploadToBlobStorageAsync(containerClient, blobsToUpload);

var blobsToCopy = await GetBlobContainerItemsForDownload(containerClient);

//var blobClient = containerClient.GetBlobClient(containerClient.AccountName);
//await DownloadBlobsAsync(blobClient, downloadDir);

foreach (var blob in blobsToCopy)
{
    // Create the BlobClient for this specific blob
    var blobClient = containerClient.GetBlobClient(blob.Name);

    // Construct the full local path to save the blob
    var localFilePath = Path.Combine(downloadDir, blob.Name);

    var folderPath = Path.GetDirectoryName(localFilePath);
    Directory.CreateDirectory(folderPath!);

    //Console.WriteLine($"Downloading {blob.Name} to {localFilePath}");

    await DownloadBlobsAsync(blobClient, localFilePath);
}

//await foreach (BlobItem blobItem in containerBlobs)
//{
//    var itemName = blobItem.Name;
//    Console.WriteLine(itemName);
//}

record BlobItemForUpload(string Name, BinaryData Content, bool CreateSubDirWithSameName = false);