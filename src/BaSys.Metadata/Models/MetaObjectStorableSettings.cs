using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.Metadata.Models
{
    public sealed class MetaObjectStorableSettings
    {
        public Guid Uid { get; set; }
        public Guid MetaObjectKindUid { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Memo { get; set; } = string.Empty;
        public long Version { get; set; }
        public bool IsActive { get; set; }
        public List<MetaObjectTable> Tables { get; set; } = new List<MetaObjectTable>();
        public MetaObjectTable Header => Tables.FirstOrDefault(x => x.IsHeader);
        public List<MetaObjectTable> TableParts => Tables.Where(x => !x.IsHeader).ToList();
    }
}
