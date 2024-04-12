using System;
using System.Collections.Generic;
using System.Text;

namespace BaSys.Metadata.Models
{
    /// <summary>
    /// Stores standard groups of metadata tree.
    /// </summary>
    public static class MetadataGroupDefaults
    {
        public static readonly MetadataGroup MetadataGroup = new MetadataGroup
        {
            Uid = new Guid("60738680-DAFD-42C0-8923-585FC7985176"),
            Title = "Metadata",
            IsStandard = true,
        };

        public static readonly MetadataGroup SystemGroup = new MetadataGroup
        {
            Uid = new Guid("AE28B333-3F36-4FEC-A276-92FCCC9B435C"),
            Title = "System",
            IsStandard = true,
        };

        public static IList<MetadataGroup> AllGroups()
        {
            var collection = new List<MetadataGroup>()
            {
                MetadataGroup,
                SystemGroup,
            }; 

            return collection;
        }
    }
}
