using BaSys.Common.Infrastructure;
using BaSys.Constructor.Abstractions;
using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.DataProviders;
using BaSys.Metadata.Models;
using BaSys.SuperAdmin.DAL.Abstractions;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Data;
using System.Xml.Linq;
using System.Linq;
using BaSys.Host.DAL.TableManagers;

namespace BaSys.Constructor.Services
{
    public sealed class MetadataKindsService : IMetadataKindsService, IDisposable
    {
        private readonly IDbConnection _connection;
        private readonly MetadataKindsProvider _provider;
        private readonly MetaObjectManager _metaObjectManager;
        private readonly ITableManagerFactory _managerFactory;
        private bool _disposed;

        public MetadataKindsService(IMainConnectionFactory connectionFactory,
            ISystemObjectProviderFactory providerFactory,
            ITableManagerFactory managerFactory)
        {
            _connection = connectionFactory.CreateConnection();
            // _provider = new MetadataKindsProvider(_connection);
            providerFactory.SetUp(_connection);
            _provider = providerFactory.Create<MetadataKindsProvider>();

            _managerFactory = managerFactory;
            _managerFactory.SetUp(_connection);

        }

        public async Task<ResultWrapper<IEnumerable<MetadataKind>>> GetCollectionAsync()
        {
            var result = new ResultWrapper<IEnumerable<MetadataKind>>();

            if (!Check())
            {
                result.Error(-1, $"Data providers is not initialized. Call SetUp method.");
                return result;
            }

            try
            {
                var items = await _provider.GetCollectionAsync(null);

                result.Success(items);

            }
            catch (Exception ex)
            {
                result.Error(-1, "Cannot get settings", $"Message: {ex.Message}, Query: {_provider.LastQuery}");
            }

            return result;
        }

        public async Task<ResultWrapper<MetadataKindSettings>> GetSettingsItemAsync(Guid uid)
        {
            var result = new ResultWrapper<MetadataKindSettings>();

            if (!Check())
            {
                result.Error(-1, $"Data providers is not initialized. Call SetUp method.");
                return result;
            }

            try
            {
                var item = await _provider.GetItemAsync(uid, null);

                if (item == null)
                {
                    result.Error(-1, "MetadataKind item not found.", $"Uid: {uid}");
                    return result;
                }

                var settings = item.ToSettings();

                result.Success(settings);

            }
            catch (Exception ex)
            {
                result.Error(-1, "Cannot get item", $"Message: {ex.Message}, Query: {_provider.LastQuery}");
            }

            return result;
        }

        public async Task<ResultWrapper<MetadataKindSettings>> GetSettingsItemByNameAsync(string name)
        {
            var result = new ResultWrapper<MetadataKindSettings>();

            if (!Check())
            {
                result.Error(-1, $"Data providers is not initialized. Call SetUp method.");
                return result;
            }

            try
            {
                var settings = await _provider.GetSettingsByNameAsync(name, null);

                if (settings == null)
                {
                    result.Error(-1, "MetadataKind item not found.", $"Name: {name}");
                    return result;
                }

                result.Success(settings);

            }
            catch (Exception ex)
            {
                result.Error(-1, "Cannot get item", $"Message: {ex.Message}, Query: {_provider.LastQuery}");
            }

            return result;
        }

        public async Task<ResultWrapper<MetadataKindSettings>> InsertSettingsAsync(MetadataKindSettings settings)
        {
            var result = new ResultWrapper<MetadataKindSettings>();

            if (!Check())
            {
                result.Error(-1, $"Data providers is not initialized. Call SetUp method.");
                return result;
            }

            _connection.Open();
            using (IDbTransaction transaction = _connection.BeginTransaction())
            {
                var savedItem = await _provider.GetItemAsync(settings.Uid, transaction);

                if (savedItem != null)
                {
                    result.Error(-1, $"Item already exists.", $"Uid: {settings.Uid}");
                    transaction.Rollback();
                    return result;
                }

                try
                {
                    var insertedCount = await _provider.InsertSettingsAsync(settings, transaction);
                    var newSettings = await _provider.GetSettingsByNameAsync(settings.Name, transaction);

                    var metaObjectManager = _managerFactory.CreateMetaObjectManager(settings.Name);
                    await metaObjectManager.CreateTableAsync(transaction);

                    transaction.Commit();

                    result.Success(newSettings);
                }
                catch (Exception ex)
                {
                    result.Error(-1, $"Cannot create item.", $"Message: {ex.Message}, Query: {_provider.LastQuery}");
                    transaction.Rollback();
                }
            }

            return result;
        }

        public async Task<ResultWrapper<int>> UpdateSettingsAsync(MetadataKindSettings settings)
        {
            var result = new ResultWrapper<int>();

            if (!Check())
            {
                result.Error(-1, $"Data providers is not initialized. Call SetUp method.");
                return result;
            }

            var savedItem = await _provider.GetItemAsync(settings.Uid, null);

            if (savedItem == null)
            {
                result.Error(-1, $"Cannot find item", $"Uid: {settings.Uid}");
                return result;
            }

            savedItem.FillBySettings(settings);
            try
            {
                var insertedCount = await _provider.UpdateAsync(savedItem, null);
                result.Success(insertedCount);
            }
            catch (Exception ex)
            {
                result.Error(-1, $"Cannot create item", $"Message: {ex.Message}, Query: {_provider.LastQuery}");
            }

            return result;
        }

        public async Task<ResultWrapper<int>> DeleteAsync(Guid uid)
        {
            var result = new ResultWrapper<int>();

            if (!Check())
            {
                result.Error(-1, $"Data providers is not initialized. Call SetUp method.");
                return result;
            }

            _connection.Open();
            using (IDbTransaction transaction = _connection.BeginTransaction())
            {
                var savedItem = await _provider.GetItemAsync(uid, transaction);

                if (savedItem == null)
                {
                    result.Error(-1, "Item not found", $"Uid: {uid}");
                    transaction.Rollback();
                    return result;
                }

                try
                {
                    var processedCount = await _provider.DeleteAsync(uid, transaction);

                    var metaObjectManager = _managerFactory.CreateMetaObjectManager(savedItem.Name);
                    await metaObjectManager.DropTableAsync(transaction);

                    transaction.Commit();

                    result.Success(processedCount);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    result.Error(-1, $"Cannot delete item", $"Uid: {uid}, Message: {ex.Message}, Query: {_provider.LastQuery}");
                }
            }

            return result;
        }

        private bool Check()
        {
            if (_provider == null)
                return false;

            return true;

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
