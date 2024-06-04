using BaSys.FluentQueries.Models;
using BaSys.Metadata.Models;

namespace BaSys.Host.DAL.ModelConfigurations;

public class AttachedFileInfoConfiguration<T> : DataModelConfiguration<AttachedFileInfo<T>>
{
    public AttachedFileInfoConfiguration(string kindName)
    {
        Table($"sys_{kindName}_attached_files");

        Column("uid").IsPrimaryKey();
        Column("metaObjectKindUid");
        Column("metaObjectUid");
        Column("objectUid");
        Column("fileName").MaxLength(260);
        Column("mimeType").MaxLength(255);
        Column("isImage");
        Column("isMainImage");
        Column("storageKind");
        Column("userId").MaxLength(36);
        Column("userName").MaxLength(100);
        Column("uploadDate");
    }
}