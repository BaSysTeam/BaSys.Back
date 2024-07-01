using BaSys.Metadata.Abstractions;
using MessagePack;

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
            IsActive = settings.IsActive;
            
            SettingsStorage = MessagePackSerializer.Serialize(settings);
        }

        public MetaObjectStorableSettings ToSettings()
        {
            var settings = MessagePackSerializer.Deserialize<MetaObjectStorableSettings>(SettingsStorage); 
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
