using BaSys.App.Abstractions;
using BaSys.Common.Infrastructure;
using BaSys.DTO.App;
using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.DataProviders;
using BaSys.Metadata.Models;
using BaSys.Metadata.Models.MenuModel;
using System.Data;

namespace BaSys.App.Services
{
    public sealed class MenusService : IMenusService, IDisposable
    {
        private readonly IDbConnection _connection;
        private readonly ISystemObjectProviderFactory _providerFactory;
        private readonly MetaMenuProvider _menuProvider;
        private readonly MetaObjectKindsProvider _kindProvider;
        private bool _disposed;

        public MenusService(IMainConnectionFactory connectionFactory,
            ISystemObjectProviderFactory providerFactory)
        {
            _connection = connectionFactory.CreateConnection();
            _providerFactory = providerFactory;
            _providerFactory.SetUp(_connection);

            _menuProvider = _providerFactory.Create<MetaMenuProvider>();
            _kindProvider = _providerFactory.Create<MetaObjectKindsProvider>();
        }

        public async Task<ResultWrapper<IEnumerable<MenuGroupDto>>> GetCollectionAsync()
        {
            var result = new ResultWrapper<IEnumerable<MenuGroupDto>>();

            try
            {
                var menuCollection = await ExecuteGetCollectionAsync();
                result.Success(menuCollection);
            }
            catch (Exception ex)
            {

                result.Error(-1, $"Cannot build menu: {ex.Message}", ex.StackTrace);
            }

            return result;
        }

        private async Task<IEnumerable<MenuGroupDto>> ExecuteGetCollectionAsync()
        {
            var metaMenus = await _menuProvider.GetCollectionAsync(true, null);
            var allKinds = await _kindProvider.GetCollectionAsync(null);

            var menuCollection = new List<MenuGroupDto>();

            foreach (var metaMenu in metaMenus)
            {
                var settings = metaMenu.ToSettings();
                foreach (var settingsItem in settings.Items)
                {
                    if (!settingsItem.IsVisible)
                    {
                        continue;
                    }

                    var menuGroup = new MenuGroupDto(settingsItem);
                    if (settingsItem.Kind == MenuSettingsGroupKinds.Group)
                    {
                        if (settingsItem.AutoFill)
                        {
                            await AutoFillMenuGroupAsync(settingsItem, allKinds, menuGroup);
                        }
                        else
                        {
                            FillMenuGroup(settingsItem, menuGroup);
                        }
                    }
                    menuCollection.Add(menuGroup);

                }

            }

            return menuCollection;
        }

        private void FillMenuGroup(MenuSettingsGroupItem settingsItem, MenuGroupDto menuGroup)
        {
            foreach (var settingsColumn in settingsItem.Items)
            {
                var menuColumn = new List<MenuItemDto>();
                foreach (var settingsSubItem in settingsColumn.Items)
                {
                    var menuSubGroup = new MenuItemDto(settingsSubItem);

                    foreach (var settingsLinkItem in settingsSubItem.Items)
                    {
                        if (!settingsLinkItem.IsVisible)
                        {
                            continue;
                        }

                        var newLinkItem = new MenuItemDto(settingsLinkItem);
                        menuSubGroup.Items.Add(newLinkItem);

                    }

                    menuColumn.Add(menuSubGroup);

                }
                menuGroup.Items.Add(menuColumn);
            }
        }

        private async Task AutoFillMenuGroupAsync(MenuSettingsGroupItem settingsItem,
            IEnumerable<MetaObjectKind> allKinds,
            MenuGroupDto menuGroup)
        {
            var kind = allKinds.FirstOrDefault(x => x.Uid == settingsItem.MetaObjectKindUidParsed);
            if (kind != null)
            {
                var metaObjectProvider = new MetaObjectStorableProvider(_connection, kind.Name);
                var metaObjects = await metaObjectProvider.GetCollectionAsync(null);

                var itemsPerColumn = settingsItem.ItemsPerColumn;
                var columns = metaObjects
                    .Where(x => x.IsActive)
                    .Select((metaObject, index) => new { metaObject, index })
                    .GroupBy(x => x.index / itemsPerColumn)
                    .Select(g => g.Select(x => x.metaObject).ToList())
                    .ToList();

                foreach (var column in columns)
                {
                    var menuColumn = new List<MenuItemDto>();
                    var menuSubGroup = new MenuItemDto();

                    foreach (var item in column)
                    {
                        var menuItem = new MenuItemDto()
                        {
                            Key = item.Uid.ToString(),
                            Label = item.Title,
                            Url = $"/app#/data-objects/{kind.Name}/{item.Name}"
                        };
                        menuSubGroup.Items.Add(menuItem);
                    }

                    menuColumn.Add(menuSubGroup);
                    menuGroup.Items.Add(menuColumn);

                }
            }
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_connection != null)
                        _connection.Dispose();
                }

                _disposed = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }


    }
}
