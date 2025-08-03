using System.IO.Compression;

namespace BulkStorageContainerUpdater.Services;

public sealed class FileSystemService : IFileSystemService
{
    public void CreateBackup(string directory)
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

    bool DoesDirectoryExist(string path)
    {
        if (!Directory.Exists(path))
        {
            return false;
        }

        return true;
    }

    public void UpdateItemUrlsContentInDirectory(string directory, string matcher, string replacement)
    {
        if (!DoesDirectoryExist(directory))
        {
            Console.WriteLine($"{directory} does not exist");
            return;
        }

        try
        {
            var directoryItems = Directory.GetDirectories(directory);
            foreach (var item in directoryItems)
            {
                var itemContents = File.ReadAllText(item);
                var newContent = itemContents.Replace(matcher, replacement);
                File.WriteAllText(item, newContent);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }
}
