using BaSys.Common.Enums;
using BaSys.DAL.Models.Admin;

namespace BaSys.Admin.DTO;

public class FileStorageConfigDto
{
    public FileStorageConfigDto()
    {
    }

    public FileStorageConfigDto(FileStorageConfig config)
    {
        Uid = config.Uid;
        StorageKind = config.StorageKind;
        S3ConnectionString = config.S3ConnectionString;
        MaxFileSizeMb = config.MaxFileSizeMb;
        IsEnabled = config.IsEnabled;
    }
    
    public Guid Uid { get; set; }
    public FileStorageKinds StorageKind { get; set; }
    public string S3ConnectionString { get; set; } = string.Empty;
    public int MaxFileSizeMb { get; set; } = 50;
    public bool IsEnabled { get; set; }

    public FileStorageConfig ToModel()
    {
        return new FileStorageConfig
        {
            Uid = Uid,
            StorageKind = StorageKind,
            S3ConnectionString = S3ConnectionString,
            MaxFileSizeMb = MaxFileSizeMb,
            IsEnabled = IsEnabled
        };
    }
}