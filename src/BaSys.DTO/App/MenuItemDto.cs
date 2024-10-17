using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.DTO.App
{
    public sealed class MenuItemDto
    {
        public string Key { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public bool Visible { get; set; } = true;
        public bool Separator { get; set; }

        public List<MenuItemDto> Items { get; set; } = new List<MenuItemDto>();
    }
}
