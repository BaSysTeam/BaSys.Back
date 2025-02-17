using BaSys.DAL.Models.Admin;
using BaSys.FluentQueries.Models;

namespace BaSys.Host.DAL.ModelConfigurations;

public class FileStorageConfigConfiguration : DataModelConfiguration<FileStorageConfig>
{
    public FileStorageConfigConfiguration()
    {
        Table("sys_filestorage_config");

        Column("uid").IsPrimaryKey();
        Column("StorageKind");
        Column("S3ConnectionString").MaxLength(512);
        Column("MaxFileSizeMb");
        Column("IsEnabled");

        OrderColumns();
    }
}