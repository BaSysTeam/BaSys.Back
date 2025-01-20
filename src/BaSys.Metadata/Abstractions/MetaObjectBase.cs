using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.Metadata.Abstractions
{
    public abstract class MetaObjectBase
    {
        public Guid Uid { get; set; }
        public Guid MetaObjectKindUid { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;      
        public string Memo { get; set; } = string.Empty;
        public long Version { get; set; }
        public bool IsActive { get; set; }
        public byte[] SettingsStorage { get; set; }

        public virtual void BeforeSave()
        {
            Version++;
        }

        public override string ToString()
        {
            return Title;
        }
    }
}
