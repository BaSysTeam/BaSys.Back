using System;
using System.Collections.Generic;
using System.Text;

namespace BaSys.Metadata.Models
{
    /// <summary>
    /// Describes group in metadata tree.
    /// </summary>
    public sealed class MetadataGroup
    {
        public Guid Uid { get; set; }
        public Guid? ParentUid { get; set; }
        public string Title { get; set; } = string.Empty;
        public string IconClass { get; set; } = string.Empty;
        public string Memo { get; set; } = string.Empty;
        public bool IsStandard { get; set; }

        public override bool Equals(object obj)
        {
            return obj is MetadataGroup group &&
                   Uid.Equals(group.Uid);
        }

        public override int GetHashCode()
        {
            return -1737426059 + Uid.GetHashCode();
        }

        public override string ToString()
        {
            return Title;
        }
    }
}
