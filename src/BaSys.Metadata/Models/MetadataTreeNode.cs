using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.Metadata.Models
{
    public class MetadataTreeNode
    {
        public Guid Uid { get; set; }
        public Guid? ParentUid { get; set; }
        public Guid? MetadataKindUid { get; set; }
        public Guid? MetadataObjectUid { get; set; }
        public string Title { get; set; }
        public string IconClass { get; set; }
        public string Memo { get; set; }
        public bool IsStandard { get; set; }
        public bool IsGroup { get; set; }
    }
}
