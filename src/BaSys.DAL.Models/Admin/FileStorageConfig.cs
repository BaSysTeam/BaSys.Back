using BaSys.Common.Enums;

namespace BaSys.DAL.Models.Admin
{
    public sealed class FileStorageConfig
    {
        public Guid Uid { get; set; } = Guid.NewGuid();
        public FileStorageKinds StorageKind { get; set; }
        public string S3ConnectionString { get; set; } = string.Empty;
        public int MaxFileSizeMb { get; set; } = 50;
        public bool IsEnabled { get; set; }
    }
}
