using BaSys.Common.Abstractions;
using System;

namespace BaSys.Metadata.Abstractions
{
    public abstract class MetaObjectBase: SystemObjectBase
    {
        public Guid MetaObjectKindUid { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;      
        public string Memo { get; set; } = string.Empty;
        public long Version { get; set; }
        public bool IsActive { get; set; }
        public byte[] SettingsStorage { get; set; }

        public override void BeforeSave()
        {
            base.BeforeSave();
            Version++;
        }

        public override string ToString()
        {
            return Title;
        }
    }
}
