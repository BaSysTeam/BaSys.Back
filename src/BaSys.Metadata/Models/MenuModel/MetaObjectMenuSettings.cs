using MessagePack;
using System;
using System.Collections.Generic;

namespace BaSys.Metadata.Models.MenuModel
{
    [MessagePackObject(keyAsPropertyName: true)]
    public sealed class MetaObjectMenuSettings
    {
        public Guid Uid { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Memo { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public long Version { get; set; }
        public List<MenuSettingsGroupItem> Items { get; set; } = new List<MenuSettingsGroupItem>();

        public void CopyFrom(MetaObjectMenuSettings source)
        {
            Name = source.Name;
            Title = source.Title;
            Memo = source.Memo;
            Version = source.Version;
            IsActive = source.IsActive;
            Items = source.Items;
        }
    }
}
