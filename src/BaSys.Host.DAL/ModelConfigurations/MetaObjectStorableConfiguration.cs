using BaSys.FluentQueries.Models;
using BaSys.Metadata.Models;

namespace BaSys.Host.DAL.ModelConfigurations;

public class MetaObjectStorableConfiguration : DataModelConfiguration<MetaObjectStorable>
{
    public MetaObjectStorableConfiguration()
    {
        Table("sys_metaobject_storable");

        Column("uid").IsPrimaryKey();
        Column("metaobjectkinduid");
        Column("title").MaxLength(100);
        Column("name").MaxLength(40).IsRequired().IsUnique();
        Column("memo").MaxLength(300).IsOptional();
        Column("version");
        Column("isactive");
        Column("settingsstorage").IsOptional();
    }
}