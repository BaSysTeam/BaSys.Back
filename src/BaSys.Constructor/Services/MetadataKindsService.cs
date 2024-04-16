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

namespace BaSys.Constructor.Services
{
    public sealed class MetadataKindsService : IMetadataKindsService
    {
        private IDbConnection? _connection;
        private MetadataKindProvider? _provider;
        private string _dbName = string.Empty;
        private bool disposedValue;

        public MetadataKindsService()
        {
        }

        public void SetUp(IDbConnection connection)
        {
            _connection = connection;
            _provider = new MetadataKindProvider(_connection);
        }

        public async Task<ResultWrapper<IList<MetadataKindSettings>>> GetSettingsCollectionAsync(IDbTransaction? transaction = null)
        {
            var result = new ResultWrapper<IList<MetadataKindSettings>>();

            if (!Check())
            {
                result.Error(-1, $"Data providers is not initialized. Call SetUp method.");
                return result;
            }

            try
            {
                var items = await _provider.GetCollectionAsync(transaction);
                var settings = items.ToList().Select(x => x.ToSettings()).ToList();

                result.Success(settings);

            }
            catch (Exception ex)
            {
                result.Error(-1, "Cannot get settings", $"Message: {ex.Message}, Query: {_provider.LastQuery}");
            }

            return result;
        }

        public async Task<ResultWrapper<MetadataKindSettings>> GetSettingsItemAsync(Guid uid, IDbTransaction? transaction = null)
        {
            var result = new ResultWrapper<MetadataKindSettings>();

            if (!Check())
            {
                result.Error(-1, $"Data providers is not initialized. Call SetUp method.");
                return result;
            }

            try
            {
                var item = await _provider.GetItemAsync(uid, transaction);

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

        public async Task<ResultWrapper<int>> InsertSettingsAsync(MetadataKindSettings settings, IDbTransaction? transaction = null)
        {
            var result = new ResultWrapper<int>();

            if (!Check())
            {
                result.Error(-1, $"Data providers is not initialized. Call SetUp method.");
                return result;
            }

            try
            {
                var insertedCount = await _provider.InsertSettingsAsync(settings, transaction);
                result.Success(insertedCount);
            }
            catch (Exception ex)
            {
                result.Error(-1, $"Cannot create item", $"Message: {ex.Message}, Query: {_provider.LastQuery}");
            }

            return result;
        }

        public async Task<ResultWrapper<int>> UpdateSettingsAsync(MetadataKindSettings settings, IDbTransaction? transaction = null)
        {
            var result = new ResultWrapper<int>();

            if (!Check())
            {
                result.Error(-1, $"Data providers is not initialized. Call SetUp method.");
                return result;
            }

            var savedItem = await _provider.GetItemAsync(settings.Uid, transaction);

            if (savedItem == null)
            {
                result.Error(-1, $"Cannot find item", $"Uid: {settings.Uid}");
                return result;
            }

            savedItem.FillBySettings(settings);
            try
            {
                var insertedCount = await _provider.UpdateAsync(savedItem, transaction);
                result.Success(insertedCount);
            }
            catch (Exception ex)
            {
                result.Error(-1, $"Cannot create item", $"Message: {ex.Message}, Query: {_provider.LastQuery}");
            }

            return result;
        }

        public async Task<ResultWrapper<int>> DeleteAsync(Guid uid, IDbTransaction? transaction = null)
        {
            var result = new ResultWrapper<int>();

            if (!Check())
            {
                result.Error(-1, $"Data providers is not initialized. Call SetUp method.");
                return result;
            }

            try
            {
                var processedCount = await _provider.DeleteAsync(uid, transaction);
                result.Success(processedCount);
            }
            catch (Exception ex)
            {
                result.Error(-1, $"Cannot delete item", $"Uid: {uid}, Message: {ex.Message}, Query: {_provider.LastQuery}");
            }

            return result;
        }

        private bool Check()
        {
            if (_provider == null)
                return false;

            return true;

        }
       
    }
}
