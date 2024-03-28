using BaSys.Metadata.DTOs;
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

        public MetadataGroup()
        {
            
        }

        public MetadataGroup(MetadataGroupDto source)
        {
            if (source == null)
                return;

            if (!string.IsNullOrEmpty(source.Uid))
            {
                Uid = Guid.Parse(source.Uid);
            }

            if (!string.IsNullOrEmpty(source.ParentUid))
            {
                Uid = Guid.Parse(source.ParentUid);
            }

            Title = source.Title;   
            IconClass = source.IconClass;
            Memo = source.Memo;
            IsStandard = source.IsStandard;
        }

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
