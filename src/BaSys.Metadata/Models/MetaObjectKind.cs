using MessagePack;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaSys.Metadata.Models
{
    public sealed class MetaObjectKind
    {
        public Guid Uid { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;    
        public string Prefix { get; set; } = string.Empty;
        public bool StoreData { get; set; }
        public bool IsReference { get; set; }
        public bool IsStandard { get; set; }
        public long Version { get; set; }
        public string Memo { get; set; } = string.Empty;
        public byte[] SettingsStorage { get; set; } = new byte[0];

        public MetaObjectKind()
        {
            
        }

        public MetaObjectKind(MetaObjectKindSettings settings)
        {
           FillBySettings(settings);
        }

        public void FillBySettings(MetaObjectKindSettings settings)
        {
            Title = settings.Title;
            Name = settings.Name;
            Prefix = settings.Prefix;
            StoreData = settings.StoreData;
            IsReference = settings.IsReference;
            IsStandard = settings.IsStandard;
            Memo = settings.Memo;

            SettingsStorage = MessagePackSerializer.Serialize(settings);
        }

        public MetaObjectKindSettings ToSettings()
        {
            var settings = MessagePackSerializer.Deserialize<MetaObjectKindSettings>(SettingsStorage); 
            settings.Uid = Uid;
            settings.Version = Version;

            return settings;
        }

        public void BeforeSave()
        {
            Version++;
        }

    }
}
