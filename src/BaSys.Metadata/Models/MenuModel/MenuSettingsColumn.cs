using MessagePack;
using System;
using System.Collections.Generic;

namespace BaSys.Metadata.Models.MenuModel
{
    [MessagePackObject(keyAsPropertyName: true)]
    public sealed class MenuSettingsColumn
    {
        public Guid Uid { get; set; }
        public List<MenuSettingsSubItem> Items { get; set; } = new List<MenuSettingsSubItem>();
    }
}
