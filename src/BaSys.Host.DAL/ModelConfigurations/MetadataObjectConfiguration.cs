using BaSys.FluentQueries.Models;
using BaSys.Metadata.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.Host.DAL.ModelConfigurations
{
    public sealed class MetadataObjectConfiguration: DataModelConfiguration<MetaObjectBase>
    {
        public MetadataObjectConfiguration(string kindNamePlural)
        {
            Table($"sys_meta_{kindNamePlural}");

            Column("uid").IsPrimaryKey();
            Column("title").MaxLength(100);
            Column("name").MaxLength(30).IsRequired().IsUnique();
            Column("memo").MaxLength(300).IsOptional();
        }
    }
}
