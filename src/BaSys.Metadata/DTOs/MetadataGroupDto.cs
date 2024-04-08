using System;
using System.Collections.Generic;
using System.Text;

namespace BaSys.Metadata.DTOs
{
    public class MetadataGroupDto
    {
        public string Uid { get; set; } = string.Empty;
        public string ParentUid { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string IconClass { get; set; } = string.Empty;
        public string Memo { get; set; } = string.Empty;
        public bool IsStandard { get; set; }
    }
}
