using MemoryPack;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaSys.Metadata.Models
{
    public sealed class MetadataKind
    {
        public Guid Uid { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;    
        public string Prefix { get; set; } = string.Empty;
        public bool StoreData { get; set; }
        public bool IsReference { get; set; }
        public bool IsStandard { get; set; }
        public string Memo { get; set; } = string.Empty;
        public byte[] SettingsStorage { get; set; } = new byte[0];

        public MetadataKind()
        {
            
        }

        public MetadataKind(MetadataKindSettings settings)
        {
           
            Uid = settings.Uid;
            Title = settings.Title;
            Name = settings.Name;
            Prefix = settings.Prefix;
            StoreData = settings.StoreData;
            IsReference = settings.IsReference;
            IsStandard = settings.IsStandard;
            Memo = settings.Memo;

            SettingsStorage = MemoryPackSerializer.Serialize(settings);

        }

        public MetadataKindSettings ToSettings()
        {
            var settings = MemoryPackSerializer.Deserialize<MetadataKindSettings>(SettingsStorage); 

            return settings;
        }

    }
}
