using MessagePack;
using System;

namespace BaSys.Metadata.Models.MenuModel
{
    [MessagePackObject(keyAsPropertyName: true)]
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
