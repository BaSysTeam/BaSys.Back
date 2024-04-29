using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.Metadata.DTOs
{
    public sealed class CreateMetaObjectDto
    {
        public Guid ParentUid { get; set; }
        public Guid MetadataKindUid { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Title { get; set; }= string.Empty;
        public string Memo { get; set; } = string.Empty;
    }
}
