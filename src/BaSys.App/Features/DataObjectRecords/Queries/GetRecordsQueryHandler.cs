using BaSys.App.Abstractions;
using BaSys.Common.Infrastructure;
using BaSys.Core.Abstractions;
using BaSys.Core.Services;
using BaSys.DAL.Models.App;
using BaSys.DTO.Core;
using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.DataProviders;
using BaSys.Metadata.Models;
using BaSys.Translation;
using System.Data;

namespace BaSys.App.Features.DataObjectRecords.Queries
{
    public sealed class GetRecordsQueryHandler: IGetRecordsQueryHandler, IDisposable
    {
        private readonly IDbConnection _connection;
        private readonly ISystemObjectProviderFactory _providerFactory;
        private readonly MetaObjectKindsProvider _kindProvider;
        private readonly IDataTypesService _dataTypesService;
        private bool _disposed;

        public GetRecordsQueryHandler(IMainConnectionFactory connectionFactory,
            ISystemObjectProviderFactory providerFactory)
        {
            _connection = connectionFactory.CreateConnection();
            _providerFactory = providerFactory;
            _providerFactory.SetUp(_connection);

            _kindProvider = _providerFactory.Create<MetaObjectKindsProvider>();

            _dataTypesService = new DataTypesService(providerFactory);
            _dataTypesService.SetUp(_connection);
        }

        public async Task<ResultWrapper<DataTableDto>> ExecuteAsync(GetRecordsQuery query)
        {
            var result = new ResultWrapper<DataTableDto>();

            try
            {
                result = await ExecuteQueryAsync(query);
            }
            catch (Exception ex)
            {
                result.Error(-1, $"Cannot get records. Message: {ex.Message}.", ex.StackTrace);
            }

            return result;
        }

        private async Task<ResultWrapper<DataTableDto>> ExecuteQueryAsync(GetRecordsQuery query)
        {
            var result = new ResultWrapper<DataTableDto>();

            var allKinds = await _kindProvider.GetCollectionAsync(null);

            var sourceKindSettings = GetSourceKindSettings(allKinds, query.SourceKindName, result);
            if (sourceKindSettings == null) return result;

            var destinationKindSettings = GetDestinationKindSettings(allKinds,
                sourceKindSettings.RecordsSettings.StorageMetaObjectKindUid,
                result);
            if (destinationKindSettings == null) return result;

            var destinationSettings = await GetMetaObjectSettings(destinationKindSettings.Name, query.DestinationObjectUid, result);
            if (destinationSettings == null) return result;

            var sourceSettings = await GetMetaObjectSettingsByName(query.SourceKindName, query.SourceObjectName, result);
            if (sourceSettings == null) return result;

            var dataTypesIndex = await _dataTypesService.GetIndexAsync(null);
            var allMetaObjects = await GetAllMetaObjectsAsync(allKinds);

            var metaObjectColumn = destinationSettings.Header.GetColumn(sourceKindSettings.RecordsSettings.StorageMetaObjectColumnUid);
            if (metaObjectColumn == null)
            {
                result.Error(-1, $"Cannot find meta object column: {sourceKindSettings.RecordsSettings.StorageMetaObjectColumnUid}");
                return result;
            }

            var objectColumn = destinationSettings.Header.GetColumn(sourceKindSettings.RecordsSettings.StorageObjectColumnUid);
            if (objectColumn == null)
            {
                result.Error(-1, $"Cannot find object column: {sourceKindSettings.RecordsSettings.StorageObjectColumnUid}");
                return result;
            }

            var provider = new DataObjectListProvider(_connection,
                destinationKindSettings,
                destinationSettings,
                allKinds,
                allMetaObjects,
                dataTypesIndex);

            var collection = await provider.GetObjectRecordsWithDisplaysAsync(metaObjectColumn.Name,
                sourceSettings.Uid,
                objectColumn.Name,
                query.SourceObjectUid,
                null);

            var dataTable = CreateDataTable(collection);
            result.Success(dataTable);

            return result;
        }

        private DataTableDto CreateDataTable(IEnumerable<DataObject> collection)
        {
            var dataTable = new DataTableDto();
            foreach (var dataObject in collection)
            {
                dataTable.AddRow(dataObject.Header);
            }
            return dataTable;
        }

        private async Task<List<MetaObjectStorable>> GetAllMetaObjectsAsync(IEnumerable<MetaObjectKind> allKinds)
        {
            var allMetaObjects = new List<MetaObjectStorable>();
            foreach (var item in allKinds)
            {
                var metaObjectProvider = new MetaObjectStorableProvider(_connection, item.Name);
                var metaObjects = await metaObjectProvider.GetCollectionAsync(null);

                allMetaObjects.AddRange(metaObjects);
            }

            return allMetaObjects;
        }

        private MetaObjectKindSettings? GetSourceKindSettings(IEnumerable<MetaObjectKind> allKinds,
            string kindName,
            ResultWrapper<DataTableDto> result)
        {
            var sourceKind = allKinds.FirstOrDefault(x => x.Name.Equals(kindName, StringComparison.InvariantCultureIgnoreCase));
            if (sourceKind == null)
            {
                result.Error(-1, $"{DictMain.CannotFindMetaObjectKind}: {kindName}");
                return null;
            }
            return sourceKind.ToSettings();
        }

        private MetaObjectKindSettings? GetDestinationKindSettings(IEnumerable<MetaObjectKind> allKinds,
            Guid destinationKindUid,
            ResultWrapper<DataTableDto> result)
        {
            var destinationKind = allKinds.FirstOrDefault(x => x.Uid == destinationKindUid);
            if (destinationKind == null)
            {
                result.Error(-1, $"{DictMain.CannotFindMetaObjectKind}: {destinationKindUid}");
                return null;
            }
            return destinationKind.ToSettings();
        }

        private async Task<MetaObjectStorableSettings?> GetMetaObjectSettings(string kind,
                                                                              Guid uid,
                                                                              ResultWrapper<DataTableDto> result)
        {
            var metaObjectProvider = new MetaObjectStorableProvider(_connection, kind);
            var metaObject = await metaObjectProvider.GetItemAsync(uid, null);

            if (metaObject == null)
            {
                result.Error(-1, $"{DictMain.CannotFindMetaObject}: {kind}.{uid}");
            }

            return metaObject?.ToSettings();
        }

        private async Task<MetaObjectStorableSettings?> GetMetaObjectSettingsByName(string kind,
                                                                                    string name,
                                                                                    ResultWrapper<DataTableDto> result)
        {
            var metaObjectProvider = new MetaObjectStorableProvider(_connection, kind);
            var metaObject = await metaObjectProvider.GetItemByNameAsync(name, null);

            if (metaObject == null)
            {
                result.Error(-1, $"{DictMain.CannotFindMetaObject}: {kind}.{name}");
            }

            return metaObject?.ToSettings();
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
