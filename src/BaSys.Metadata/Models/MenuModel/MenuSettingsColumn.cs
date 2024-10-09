using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.Metadata.Models.MenuModel
{
    public sealed class MenuSettingsColumn
    {
        public Guid Uid { get; set; }
        public List<MenuSettingsSubItem> Items { get; set; } = new List<MenuSettingsSubItem>();
    }
}
