namespace BulkStorageContainerUpdater.Services
{
    public interface IFileSystemService
    {
        void CreateBackup(string directory);
        void UpdateItemUrlsContentInDirectory(string directory, string matcher, string replacement);
    }
}