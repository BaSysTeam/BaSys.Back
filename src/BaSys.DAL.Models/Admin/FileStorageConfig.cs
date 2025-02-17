using BaSys.Common.Abstractions;
using BaSys.Common.Enums;

namespace BaSys.DAL.Models.Admin
{
    public sealed class FileStorageConfig: SystemObjectBase
    {
        public FileStorageKinds StorageKind { get; set; }
        public string S3ConnectionString { get; set; } = string.Empty;
        public int MaxFileSizeMb { get; set; } = 50;
        public bool IsEnabled { get; set; }

        public FileStorageConfig()
        {
            Uid = Guid.NewGuid();
        }
    }
}
