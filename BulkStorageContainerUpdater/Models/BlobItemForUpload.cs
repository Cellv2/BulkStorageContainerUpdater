namespace BulkStorageContainerUpdater.Models;

public sealed record BlobItemForUpload(string Name, BinaryData Content, bool CreateSubDirWithSameName = false);
