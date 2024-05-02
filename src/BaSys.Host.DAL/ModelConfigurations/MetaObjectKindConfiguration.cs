using BaSys.FluentQueries.Models;
using BaSys.Metadata.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.Host.DAL.ModelConfigurations
{
    public sealed class MetaObjectKindConfiguration: DataModelConfiguration<MetaObjectKind>
    {
        public MetaObjectKindConfiguration()
        {
            Table("sys_meta_object_kinds");

            Column("uid").IsPrimaryKey();
            Column("title").MaxLength(100);
            Column("name").MaxLength(40).IsRequired().IsUnique();
            Column("prefix").MaxLength(4).IsRequired().IsUnique();
            Column("memo").MaxLength(300).IsOptional();
        }
    }
}
