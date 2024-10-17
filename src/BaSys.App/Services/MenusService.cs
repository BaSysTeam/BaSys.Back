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
        private bool _disposed;

        public MenusService(IMainConnectionFactory connectionFactory,
            ISystemObjectProviderFactory providerFactory)
        {
            _connection = connectionFactory.CreateConnection();
            _providerFactory = providerFactory;
            _providerFactory.SetUp(_connection);

            _menuProvider = _providerFactory.Create<MetaMenuProvider>();
        }

        public async Task<ResultWrapper<IEnumerable<MenuItemDto>>> GetCollectionAsync()
        {
            var result = new ResultWrapper<IEnumerable<MenuItemDto>>();

            var metaMenus = await _menuProvider.GetCollectionAsync(true, null);

            var menuCollection = new List<MenuItemDto>();

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
                            var newSeparator = new MenuItemDto()
                            {
                                Separator = true,
                                Key = settingsItem.Uid.ToString()
                            };
                            menuCollection.Add(newSeparator);
                            break;
                        case MenuSettingsGroupKinds.Link:
                            var newLink = new MenuItemDto() 
                            { 
                                Label = settingsItem.Title,
                                Icon = settingsItem.IconClass,
                                Url = settingsItem.Url,
                                Key = settingsItem.Uid.ToString()
                            };
                            menuCollection.Add(newLink);
                            break;
                        case MenuSettingsGroupKinds.Group:
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
