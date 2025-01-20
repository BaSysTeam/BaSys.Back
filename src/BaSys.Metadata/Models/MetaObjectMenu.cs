using BaSys.Metadata.Abstractions;
using BaSys.Metadata.Defaults;
using BaSys.Metadata.Models.MenuModel;
using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.Metadata.Models
{
    public sealed class MetaObjectMenu: MetaObjectBase
    {
        public void FillBySettings(MenuSettings settings)
        {
            MetaObjectKindUid = MetaObjectKindDefaults.Menu.Uid;
            Title = settings.Title;
            Name = settings.Name;
            Memo = settings.Memo;
            IsActive = settings.IsActive;

            SettingsStorage = MessagePackSerializer.Serialize(settings);
        }

        public MenuSettings ToSettings()
        {
            var settings = MessagePackSerializer.Deserialize<MenuSettings>(SettingsStorage);
            settings.Uid = Uid;
            settings.Version = Version;

            return settings;
        }
    }
}
