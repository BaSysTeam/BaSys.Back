﻿using MessagePack;
using System;
using System.Collections.Generic;

namespace BaSys.Metadata.Models.MenuModel
{
    [MessagePackObject(keyAsPropertyName: true)]
    public sealed class MenuSettingsGroupItem
    {
        public Guid Uid { get; set; }
        public MenuSettingsGroupKinds Kind { get; set; } = MenuSettingsGroupKinds.Group;
        public string Title { get; set; } = string.Empty;
        public string IconClass { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public bool IsVisible { get; set; }
        public bool AutoFill { get; set; }
        public int ItemsPerColumn { get; set; } = 10;
        public string MetaObjectKindUid { get; set; }
        public List<MenuSettingsColumn> Items { get; set; } = new List<MenuSettingsColumn>();

        public Guid MetaObjectKindUidParsed
        {
            get
            {
                if (Guid.TryParse(MetaObjectKindUid, out var uid))
                {
                    return uid;
                }
                return Guid.Empty;
            }
        }
    }
}

