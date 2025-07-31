using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkStorageContainerUpdater.Services;

public sealed  class FileSystemService
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
