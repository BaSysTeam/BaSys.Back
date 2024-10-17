using BaSys.App.Abstractions;
using BaSys.Common.Infrastructure;
using BaSys.Core.Abstractions;
using BaSys.DTO.App;
using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.DataProviders;
using BaSys.Logging.Abstractions.Abstractions;
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

                    switch (settingsItem.Kind)
                    {
                        case MenuSettingsGroupKinds.Separator:
                            var newSeparator = new MenuGroupDto()
                            {
                                Separator = true,
                                Key = settingsItem.Uid.ToString()
                            };
                            menuCollection.Add(newSeparator);
                            break;
                        case MenuSettingsGroupKinds.Link:
                            var newLink = new MenuGroupDto()
                            {
                                Label = settingsItem.Title,
                                Icon = settingsItem.IconClass,
                                Url = settingsItem.Url,
                                Key = settingsItem.Uid.ToString()
                            };
                            menuCollection.Add(newLink);
                            break;
                        case MenuSettingsGroupKinds.Group:
                            var menuGroup = new MenuGroupDto()
                            {
                                Label = settingsItem.Title,
                                Icon = settingsItem.IconClass,
                                Url = null,
                                Key = settingsItem.Uid.ToString()
                            };

                            if (settingsItem.AutoFill)
                            {
                                var kind = allKinds.FirstOrDefault(x => x.Uid == settingsItem.MetaObjectKindUidParsed);
                                if (kind != null)
                                {
                                    var metaObjectProvider = new MetaObjectStorableProvider(_connection, kind.Name);
                                    var metaObjects = await metaObjectProvider.GetCollectionAsync(null);

                                    var itemsPerColumn = settingsItem.ItemsPerColumn;
                                    var columns = metaObjects
                                        .Where(x=>x.IsActive)
                                        .Select((metaObject, index) => new { metaObject, index })
                                        .GroupBy(x => x.index / itemsPerColumn)
                                        .Select(g => g.Select(x => x.metaObject).ToList())
                                        .ToList();

                                    foreach(var column in columns)
                                    {
                                        var menuColumn = new List<MenuItemDto>();
                                        var menuSubGroup = new MenuItemDto();

                                        foreach(var item in column)
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

                            menuCollection.Add(menuGroup);
                            break;
                        default:
                            break;
                    }
                }

            }

            result.Success(menuCollection);

            return result;
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

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
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
