using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.Metadata.Models.MenuModel
{
    public sealed class MenuSettingsSubItem
    {
        public Guid Uid { get; set; }
        public string Title { get; set; } = string.Empty;
        public bool IsVisible { get; set; }
        public List<MenuSettingsLinkItem> Items { get; set; } = new List<MenuSettingsLinkItem>();
    }
}
