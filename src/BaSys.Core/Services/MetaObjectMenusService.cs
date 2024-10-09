using BaSys.Common.Infrastructure;
using BaSys.Core.Abstractions;
using BaSys.DTO.Constructor;
using BaSys.DTO.Metadata;
using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.DataProviders;
using BaSys.Logging.Abstractions.Abstractions;
using BaSys.Metadata.Models;
using BaSys.Metadata.Models.MenuModel;
using BaSys.Translation;
using System.Data;

namespace BaSys.Core.Services
{
    public class MetaObjectMenusService : IMetaObjectMenusService, IDisposable
    {
        private readonly IDbConnection _connection;
        private readonly MetaObjectKindsProvider _kindsProvider;
        private readonly ILoggerService _logger;
        private readonly ISystemObjectProviderFactory _providerFactory;
        private readonly IDataTypesService _dataTypesService;
        private readonly MetaObjectMenuProvider _menuProvider;
        private bool _disposed;

        public MetaObjectMenusService(IMainConnectionFactory connectionFactory,
            ISystemObjectProviderFactory providerFactory,
            ILoggerService logger)
        {
            _connection = connectionFactory.CreateConnection();

            _providerFactory = providerFactory;
            _providerFactory.SetUp(_connection);

            _kindsProvider = _providerFactory.Create<MetaObjectKindsProvider>();
            _menuProvider = _providerFactory.Create<MetaObjectMenuProvider>();

            _dataTypesService = new DataTypesService(providerFactory);
            _dataTypesService.SetUp(_connection);

            _logger = logger;
        }

        public async Task<ResultWrapper<MetaObjectListDto>> GetKindListAsync()
        {
            var result = new ResultWrapper<MetaObjectListDto>();

            try
            {
                var collection = await _menuProvider.GetCollectionAsync(null);
                var listDto = new MetaObjectListDto
                {
                    Title = "Menu",
                    MetaObjectKindUid = MetaObjectKindDefaults.Menu.Uid.ToString(),
                    Items = collection.Select(x => new MetaObjectDto(x)).ToList()
                };

                result.Success(listDto);
            }
            catch (Exception ex)
            {
                result.Error(-1, $"Cannot get data: {ex.Message}", ex.StackTrace);
            }

            return result;
        }

        public async Task<ResultWrapper<MetaObjectMenuSettings>> GetSettingsItemAsync(string objectName)
        {
            var result = new ResultWrapper<MetaObjectMenuSettings>();
            try
            {
                var item = await _menuProvider.GetItemByNameAsync(objectName, null);
                if (item != null)
                {
                    result.Success(item.ToSettings());
                }
                else
                {
                    result.Error(-1, $"{DictMain.CannotFindItem} Menu.{objectName}");
                }
            }
            catch (Exception ex)
            {
                result.Error(-1, $"{DictMain.CannotFindItem} Menu.{objectName}: {ex.Message}", ex.StackTrace);
            }

            return result;
        }

        public async Task<ResultWrapper<int>> CreateAsync(MetaObjectMenuSettings settings)
        {
            var result = new ResultWrapper<int>();

            try
            {
                var insertedCount = await _menuProvider.InsertSettingsAsync(settings, null);
                result.Success(insertedCount);
            }
            catch (Exception ex)
            {
                result.Error(-1, $"{DictMain.CannotCreateItem}: {ex.Message}", ex.StackTrace);
            }

            return result;
        }

        public async Task<ResultWrapper<int>> UpdateSettingsItemAsync(MetaObjectMenuSettings settings)
        {
            var result = new ResultWrapper<int>();

            try
            {
                var savedSettings = await _menuProvider.GetSettingsItemAsync(settings.Uid, null);
                if (savedSettings == null)
                {
                    result.Error(-1, $"{DictMain.CannotFindItem} Menu.{settings.Name}: {settings.Uid}");
                }

                savedSettings.CopyFrom(settings);
                var insertedCount = await _menuProvider.UpdateSettingsAsync(savedSettings, null);
            }
            catch (Exception ex)
            {
                result.Error(-1, $"{DictMain.CannotDeleteItem}: {ex.Message}", ex.StackTrace);
            }

            return result;
        }


        public async Task<ResultWrapper<int>> DeleteAsync(string objectName)
        {
            var result = new ResultWrapper<int>();

            try
            {
                result = await ExecuteDeleteAsync(objectName);
            }
            catch (Exception ex)
            {
                result.Error(-1, $"{DictMain.CannotDeleteItem} Menu.{objectName}: {ex.Message}", ex.StackTrace);
            }


            return result;
        }

        private async Task<ResultWrapper<int>> ExecuteDeleteAsync(string objectName)
        {
            var result = new ResultWrapper<int>();

            var item = await _menuProvider.GetItemByNameAsync(objectName, null);
            if (item == null)
            {
                result.Error(-1, $"{DictMain.CannotFindItem} Menu.{objectName}");
                return result;
            }

            var deletedCount = await _menuProvider.DeleteAsync(item.Uid, null);
            result.Success(deletedCount);

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
