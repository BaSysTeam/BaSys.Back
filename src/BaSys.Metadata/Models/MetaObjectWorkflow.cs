using BaSys.Metadata.Abstractions;
using BaSys.Metadata.Defaults;
using BaSys.Metadata.Models.MenuModel;
using BaSys.Metadata.Models.WorkflowModel;
using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.Metadata.Models
{
    internal class MetaObjectWorkflow: MetaObjectBase
    {
        public void FillBySettings(WorkflowSettings settings)
        {
            MetaObjectKindUid = MetaObjectKindDefaults.Menu.Uid;
            Title = settings.Title;
            Name = settings.Name;
            Memo = settings.Memo;
            IsActive = settings.IsActive;

            SettingsStorage = MessagePackSerializer.Serialize(settings);
        }

        public WorkflowSettings ToSettings()
        {
            var settings = MessagePackSerializer.Deserialize<WorkflowSettings>(SettingsStorage);
            settings.Uid = Uid;
            settings.Version = Version;

            return settings;
        }
    }
}
