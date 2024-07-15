using MessagePack;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaSys.Metadata.Models
{
    [MessagePackObject(keyAsPropertyName: true)]
    public sealed class MetaObjectKindSettings
    {
        public Guid Uid { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Prefix { get; set; } = string.Empty;
        public bool StoreData { get; set; }
        public bool IsReference { get; set; }
        public bool AllowAttacheFiles { get; set; }
        public bool IsStandard { get; set; }
        public long Version { get; set; }
        public string Memo { get; set; } = string.Empty;
        public string IconClass { get; set; } = string.Empty;
        public string OrderByExpression { get; set;} = string.Empty;    

        public List<MetaObjectKindStandardColumn> StandardColumns { get; set; }   = new List<MetaObjectKindStandardColumn>();
        public List<Guid> AvailableRoles { get; set; } = new List<Guid>();

    }
}
