using BaSys.FluentQueries.Models;
using BaSys.Metadata.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.Host.DAL.ModelConfigurations
{
    public sealed class MetadataTreeNodeConfiguration : DataModelConfiguration<MetadataTreeNode>
    {
        public MetadataTreeNodeConfiguration()
        {
            Table("sys_metadata_tree_nodes");

            Column("Uid").IsPrimaryKey();
            Column("ParentUid").IsOptional();
            Column("MetadataKindUid").IsOptional();
            Column("MetadataObjectUid").IsOptional();
            Column("Title").MaxLength(100).IsRequired();
            Column("Memo").MaxLength(300).IsOptional();
            Column("IconClass").MaxLength(20).IsOptional();
            Column("IsStandard").IsRequired();
            Column("IsGroup").IsRequired();
        }
    }
}
