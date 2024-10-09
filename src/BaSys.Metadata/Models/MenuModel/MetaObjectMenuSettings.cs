using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.Metadata.Models.MenuModel
{
    public sealed class MetaObjectMenuSettings
    {
        public Guid Uid { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Memo { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public List<MenuSettingsGroupItem> Items { get; set; } = new List<MenuSettingsGroupItem>();
    }
}
