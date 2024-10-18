using BaSys.Metadata.Models.MenuModel;

namespace BaSys.DTO.App
{
    public sealed class MenuGroupDto
    {
        public string Key { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public string? Url { get; set; }
        public bool Visible { get; set; } = true;
        public bool Separator { get; set; }

        public List<List<MenuItemDto>> Items { get; set; } = new List<List<MenuItemDto>>();

        public MenuGroupDto()
        {

        }

        public MenuGroupDto(MenuSettingsGroupItem source)
        {
            Key = source.Uid.ToString();
            Visible = source.IsVisible;

            switch (source.Kind)
            {
                case MenuSettingsGroupKinds.Separator:
                    Separator = true;
                    break;

                case MenuSettingsGroupKinds.Link:
                    Label = source.Title;
                    Icon = source.IconClass;
                    Url = source.Url;
                    break;

                case MenuSettingsGroupKinds.Group:
                    Label = source.Title;
                    Icon = source.IconClass;
                    break;
                default:
                    throw new ArgumentException($"Unknown MenuSettingsGroupItemKind: {source.Kind}");
                  
            }
        }
    }
}

