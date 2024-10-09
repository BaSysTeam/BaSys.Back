using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.Metadata.Models.MenuModel
{
    public sealed class MenuSettingsGroupItem
    {
        public Guid Uid { get; set; }
        public MenuSettingsGroupKinds Kind { get; set; } = MenuSettingsGroupKinds.Group;
        public string Title { get; set; } = string.Empty;
        public string IconClass { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public bool IsVisible { get; set; }
        public List<MenuSettingsColumn> Items { get; set; } = new List<MenuSettingsColumn>();
    }
}
