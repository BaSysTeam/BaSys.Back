using MessagePack;
using System;
using System.Collections.Generic;

namespace BaSys.Metadata.Models.MenuModel
{
    [MessagePackObject(keyAsPropertyName: true)]
    public sealed class MenuSettingsSubItem
    {
        public Guid Uid { get; set; }
        public string Title { get; set; } = string.Empty;
        public bool IsVisible { get; set; }
        public List<MenuSettingsLinkItem> Items { get; set; } = new List<MenuSettingsLinkItem>();
    }
}
