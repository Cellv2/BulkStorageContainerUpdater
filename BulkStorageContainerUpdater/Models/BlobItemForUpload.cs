using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkStorageContainerUpdater.Models;

public sealed record BlobItemForUpload(string Name, BinaryData Content, bool CreateSubDirWithSameName = false);
