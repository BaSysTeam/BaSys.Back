using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.Metadata.Models.MenuModel
{
    public sealed class MenuSettingsLinkItem
    {
        public Guid Uid { get; set; }
        public MenuSettingsLinkKinds Kind { get; set; }
        public string Title { get; set; } = string.Empty;
        public string IconClass { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public bool IsVisible { get; set; }
    }
}
