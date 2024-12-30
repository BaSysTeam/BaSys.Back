using BaSys.Metadata.Models;
using BaSys.Metadata.Models.MenuModel;
using System;
using System.Collections.Generic;

namespace BaSys.Metadata.Helpers
{
    public sealed class DefaultMenuBuilder
    {
        public static MenuSettings Build(IEnumerable<MetaObjectKindSettings> kindSettings)
        {
            var menuSettings = new MenuSettings();
            menuSettings.Title = "Main menu";
            menuSettings.Name = "main";
            menuSettings.IsActive = true;

            foreach(var settings in kindSettings)
            {
                if (settings.IsStandard)
                {
                    continue;
                }

                if (!settings.StoreData)
                {
                    continue;
                }

                var groupItem = new MenuSettingsGroupItem();
                groupItem.Kind = MenuSettingsGroupKinds.Group;
                groupItem.AutoFill = true;
                groupItem.Title = settings.Title;
                groupItem.ItemsPerColumn = 10;
                groupItem.MetaObjectKindUid = settings.Uid.ToString();
                groupItem.Uid = Guid.NewGuid();
                groupItem.IsVisible = true;

                menuSettings.Items.Add(groupItem);
            }

            return menuSettings;
        }
    }
}
