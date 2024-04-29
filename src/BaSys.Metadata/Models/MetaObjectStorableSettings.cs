using System;
using System.Collections.Generic;
using System.Linq;
using MemoryPack;

namespace BaSys.Metadata.Models
{
    [MemoryPackable]
    public sealed partial class MetaObjectStorableSettings
    {
        public Guid Uid { get; set; }
        public Guid MetaObjectKindUid { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Memo { get; set; } = string.Empty;
        public long Version { get; set; }
        public bool IsActive { get; set; }
        public List<MetaObjectTable> Tables { get; set; } = new ();
        public MetaObjectTable Header => Tables.FirstOrDefault(x => x.IsHeader);
        public List<MetaObjectTable> TableParts => Tables.Where(x => !x.IsHeader).ToList();
    }
}
