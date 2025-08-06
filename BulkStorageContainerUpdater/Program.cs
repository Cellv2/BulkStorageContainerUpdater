using Azure.Storage.Blobs;
using BulkStorageContainerUpdater.Services;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;
using System.CommandLine;
using System.ComponentModel;

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

Option<bool> shouldCreateTestBlobItems = new("--create-test-items")
{
    Description = "Whether to upload test items into random folders in the specified storage account container"
};

RootCommand rootCommand = new("Sample app for System.CommandLine");
rootCommand.Options.Add(shouldCreateTestBlobItems);

ParseResult parseResult = rootCommand.Parse(args);
parseResult.Invoke();

//var app = new CommandApp


// https://spectreconsole.net/cli/getting-started
internal sealed class CommandThing : Command<CommandThing.Settings>
{
    public override int Execute(CommandContext context, Settings settings)
    {
        throw new NotImplementedException();
    }

    public sealed class Settings : CommandSettings
    {
        [CommandOption("--createTestBlobItems")]
        [DefaultValue(false)]
        public bool ShouldCreateTestBlobItems { get; init; }
    }
}



//var services = new ServiceCollection();
//services.AddTransient<IFileSystemService, FileSystemService>();
//services.AddTransient<IAzureBlobService, AzureBlobService>();

//var serviceProvider = services.BuildServiceProvider();

//var azureBlobService = serviceProvider.GetRequiredService<IAzureBlobService>();
//var fileSystemService = serviceProvider.GetRequiredService<IFileSystemService>();


//// https://learn.microsoft.com/en-us/dotnet/standard/commandline/get-started-tutorial


//var connectionString = "";

//var targetContainerName = "downloadtest";

//var downloadDir = "D:\\dev\\blobTest";

////var blobServiceClient = new BlobServiceClient(connectionString);

////var blobContainers = blobServiceClient.GetBlobContainersAsync();


//var containerClient = new BlobContainerClient(connectionString, targetContainerName);
//var containerBlobs = containerClient.GetBlobsAsync();

////var blobsToUpload = CreateBlobItemsForUpload(50);
////await UploadToBlobStorageAsync(containerClient, blobsToUpload);

//var blobsToCopy = await azureBlobService.GetBlobContainerItemsForDownload(containerClient);

////var blobClient = containerClient.GetBlobClient(containerClient.AccountName);
////await DownloadBlobsAsync(blobClient, downloadDir);

//foreach (var blob in blobsToCopy)
//{
//    // Create the BlobClient for this specific blob
//    var blobClient = containerClient.GetBlobClient(blob.Name);

//    // Construct the full local path to save the blob
//    var localFilePath = Path.Combine(downloadDir, blob.Name);

//    var folderPath = Path.GetDirectoryName(localFilePath);
//    Directory.CreateDirectory(folderPath!);

//    //Console.WriteLine($"Downloading {blob.Name} to {localFilePath}");

//    await azureBlobService.DownloadBlobsAsync(blobClient, localFilePath);
//}

////await foreach (BlobItem blobItem in containerBlobs)
////{
////    var itemName = blobItem.Name;
////    Console.WriteLine(itemName);
////}

