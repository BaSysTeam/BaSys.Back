using BaSys.Metadata.Models.MenuModel;

namespace BaSys.DTO.App
{
    public sealed class MenuItemDto
    {
        public string Key { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public string? Url { get; set; }
        public bool Visible { get; set; } = true;
        public bool Separator { get; set; }

        public List<MenuItemDto> Items { get; set; } = new List<MenuItemDto>();

        public MenuItemDto()
        {

        }

        public MenuItemDto(MenuSettingsSubItem source)
        {
            Key = source.Uid.ToString();
            Label = source.Title;
            Visible = source.IsVisible;
        }

        public MenuItemDto(MenuSettingsLinkItem source)
        {
            Key = source.Uid.ToString();
            Visible = source.IsVisible;

            if (source.Kind == MenuSettingsLinkKinds.Separator)
            {
                Separator = true;
            }
            else if (source.Kind == MenuSettingsLinkKinds.Link)
            {
                Label = source.Title;
                Icon = source.IconClass;
                Url = source.Url;
            }
            else
            {
                throw new ArgumentException($"Unknown MenuSettingsLinkKind: {source.Kind}");
            }
        }
    }
}
