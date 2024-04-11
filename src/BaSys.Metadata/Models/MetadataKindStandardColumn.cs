using System;
using System.Collections.Generic;
using System.Text;

namespace BaSys.Metadata.Models
{
    public sealed class MetadataKindStandardColumn
    {
        public Guid Uid { get; set; } = Guid.NewGuid();
        public string Title { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public Guid DataTypeUid { get; set; }
        public bool IsPrimaryKey { get; set; }
        public string Memo { get; set; }
    }
}
