using BaSys.Metadata.Abstractions;
using MemoryPack;

namespace BaSys.Metadata.Models
{
    public sealed class MetaObjectStorable: MetaObjectBase
    {
        public void FillBySettings(MetaObjectStorableSettings settings)
        {
            MetaObjectKindUid = settings.MetaObjectKindUid;
            Title = settings.Title;
            Name = settings.Name;
            Memo = settings.Memo;
            Version = settings.Version;
            IsActive = settings.IsActive;
            
            SettingsStorage = MemoryPackSerializer.Serialize(settings);
        }

        public MetaObjectStorableSettings ToSettings()
        {
            var settings = MemoryPackSerializer.Deserialize<MetaObjectStorableSettings>(SettingsStorage); 
            settings.Uid = Uid;

            return settings;
        }
    }
}
