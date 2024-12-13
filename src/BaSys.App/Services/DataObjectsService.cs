using BaSys.App.Abstractions;
using BaSys.Common.Infrastructure;
using BaSys.Core.Abstractions;
using BaSys.Core.Services;
using BaSys.Core.Services.RecordsBuilder;
using BaSys.DAL.Models.App;
using BaSys.DTO.App;
using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.DataProviders;
using BaSys.Logging.Abstractions.Abstractions;
using BaSys.Logging.InMemory;
using BaSys.Metadata.Helpers;
using BaSys.Metadata.Models;
using BaSys.Translation;
using Humanizer;
using MongoDB.Bson;
using System.Data;
using System.Security.AccessControl;
using System.Text.Json;


namespace BaSys.App.Services
{
    public sealed class DataObjectsService : IDataObjectsService, IDisposable
    {
        private readonly IDbConnection _connection;
        private readonly MetaObjectKindsProvider _kindProvider;
        private readonly IDataTypesService _dataTypesService;
        private readonly ILoggerService _logger;
        private bool _disposed;

        public DataObjectsService(IMainConnectionFactory connectionFactory,
            ISystemObjectProviderFactory providerFactory,
            ILoggerService logger)
        {
            _connection = connectionFactory.CreateConnection();

            providerFactory.SetUp(_connection);
            _kindProvider = providerFactory.Create<MetaObjectKindsProvider>();

            _dataTypesService = new DataTypesService(providerFactory);
            _dataTypesService.SetUp(_connection);

            _logger = logger;

        }

        public async Task<ResultWrapper<DataObjectListDto>> GetCollectionAsync(string kindName, string objectName)
        {
            var result = new ResultWrapper<DataObjectListDto>();

            var allKinds = await _kindProvider.GetCollectionAsync(null);
            var kind = allKinds.FirstOrDefault(x => x.Name.Equals(kindName, StringComparison.OrdinalIgnoreCase));

            if (kind == null)
            {
                result.Error(-1, $"{DictMain.CannotFindMetaObjectKind}: {kindName}");
                return result;
            }

            var objectKindSettings = kind.ToSettings();

            var metaObjectProvider = new MetaObjectStorableProvider(_connection, objectKindSettings.Name);
            var metaObject = await metaObjectProvider.GetItemByNameAsync(objectName, null);

            if (metaObject == null)
            {
                result.Error(-1, $"{DictMain.CannotFindMetaObject}: {kindName}.{objectName}");
                return result;
            }

            var metaObjectSettings = metaObject.ToSettings();
            var dataTypesIndex = await _dataTypesService.GetIndexAsync(null);

            var allMetaObjects = new List<MetaObjectStorable>();

            foreach (var item in allKinds)
            {
                metaObjectProvider = new MetaObjectStorableProvider(_connection, item.Name);
                var metaObjects = await metaObjectProvider.GetCollectionAsync(null);

                allMetaObjects.AddRange(metaObjects);
            }

            var provider = new DataObjectListProvider(_connection, objectKindSettings, metaObjectSettings, allKinds, allMetaObjects, dataTypesIndex);


            try
            {
                var collection = await provider.GetCollectionWithDisplaysAsync(null);
                var listDto = new DataObjectListDto(objectKindSettings, metaObjectSettings, collection, dataTypesIndex.ToList());

                result.Success(listDto);
            }
            catch (Exception ex)
            {
                result.Error(-1, $"Cannot get collection {kindName}.{objectName}", $"Message: {ex.Message}, Query: {provider.LastQuery}");
            }


            return result;
        }

        public async Task<ResultWrapper<DataObjectWithMetadataDto>> GetItemAsync(string kindName, string objectName, string uid)
        {
            var result = new ResultWrapper<DataObjectWithMetadataDto>();

            var allKinds = await _kindProvider.GetCollectionAsync(null);
            var kind = allKinds.FirstOrDefault(x => x.Name.Equals(kindName, StringComparison.OrdinalIgnoreCase));

            if (kind == null)
            {
                result.Error(-1, $"{DictMain.CannotFindMetaObjectKind}: {kindName}");
                return result;
            }

            var objectKindSettings = kind.ToSettings();

            var metaObjectProvider = new MetaObjectStorableProvider(_connection, objectKindSettings.Name);
            var metaObject = await metaObjectProvider.GetItemByNameAsync(objectName, null);

            if (metaObject == null)
            {
                result.Error(-1, $"{DictMain.CannotFindMetaObject}: {kindName}.{objectName}");
                return result;
            }

            var metaObjectSettings = metaObject.ToSettings();
            var dataTypesIndex = await _dataTypesService.GetIndexAsync(null);

            var allMetaObjects = new List<MetaObjectStorable>();

            foreach (var item in allKinds)
            {
                metaObjectProvider = new MetaObjectStorableProvider(_connection, item.Name);
                var metaObjects = await metaObjectProvider.GetCollectionAsync(null);

                allMetaObjects.AddRange(metaObjects);
            }

            // var provider = new DataObjectProvider(_connection, objectKindSettings, metaObjectSettings, dataTypesIndex);
            var provider = new DataObjectListProvider(_connection, objectKindSettings, metaObjectSettings, allKinds, allMetaObjects, dataTypesIndex);


            try
            {
                var item = await provider.GetItemWithDisplaysAsync(uid, null);

                DataObjectWithMetadataDto dto;
                if (item != null)
                {
                    dto = new DataObjectWithMetadataDto(objectKindSettings, metaObjectSettings, item);
                }
                else
                {
                    dto = new DataObjectWithMetadataDto(objectKindSettings, metaObjectSettings, new DataObject(metaObjectSettings, dataTypesIndex));
                }
                dto.DataTypes = (await _dataTypesService.GetAllDataTypesAsync(null)).Select(x => new DTO.Core.DataTypeDto(x)).ToList();
                result.Success(dto);

            }
            catch (Exception ex)
            {
                result.Error(-1, $"Cannot get item.", $"Message: {ex.Message}, query: {provider.LastQuery}");
            }

            return result;
        }

        public async Task<ResultWrapper<DataObjectDetailsTableDto>> GetDetailsTableAsync(string kindName, string objectName, string uid, string tableName)
        {
            var result = new ResultWrapper<DataObjectDetailsTableDto>();

            var allKinds = await _kindProvider.GetCollectionAsync(null);
            var kind = allKinds.FirstOrDefault(x => x.Name.Equals(kindName, StringComparison.OrdinalIgnoreCase));

            if (kind == null)
            {
                result.Error(-1, $"{DictMain.CannotFindMetaObjectKind}: {kindName}");
                return result;
            }

            var objectKindSettings = kind.ToSettings();
            var metaObjectProvider = new MetaObjectStorableProvider(_connection, objectKindSettings.Name);
            var metaObject = await metaObjectProvider.GetItemByNameAsync(objectName, null);

            if (metaObject == null)
            {
                result.Error(-1, $"{DictMain.CannotFindMetaObject}: {kindName}.{objectName}");
                return result;
            }

            var metaObjectSettings = metaObject.ToSettings();

            var detailTableSettings = metaObjectSettings.DetailTables.FirstOrDefault(x => x.Name.Equals(tableName, StringComparison.InvariantCultureIgnoreCase));

            if (detailTableSettings == null)
            {
                result.Error(-1, $"Cannot find detail table: {kindName}.{objectName}.details.{tableName}");
                return result;
            }

            var dataTypesIndex = await _dataTypesService.GetIndexAsync(null);

            var allMetaObjects = new List<MetaObjectStorable>();

            foreach (var item in allKinds)
            {
                metaObjectProvider = new MetaObjectStorableProvider(_connection, item.Name);
                var metaObjects = await metaObjectProvider.GetCollectionAsync(null);

                allMetaObjects.AddRange(metaObjects);
            }

            var provider = new DataObjectDetailsTableProvider(_connection,
                objectKindSettings,
                metaObjectSettings,
                detailTableSettings,
                allKinds,
                allMetaObjects,
                dataTypesIndex);

            try
            {
                var table = await provider.GetTableWithDisplaysAsync(uid, null);
                var tableDto = new DataObjectDetailsTableDto(table);
                result.Success(tableDto);
            }
            catch (Exception ex)
            {
                result.Error(-1, $"Cannot get table.", $"Message: {ex.Message}, Query: {provider.LastQuery}");
            }

            return result;
        }

        public async Task<ResultWrapper<string>> InsertAsync(DataObjectSaveDto dto)
        {
            var result = new ResultWrapper<string>();

            _connection.Open();
            using (IDbTransaction transaction = _connection.BeginTransaction())
            {
                var allKinds = await _kindProvider.GetCollectionAsync(transaction);
                var kind = allKinds.FirstOrDefault(x => x.Uid == dto.MetaObjectKindUid);

                if (kind == null)
                {
                    result.Error(-1, $"{DictMain.CannotFindMetaObjectKind}", $"MetaObjectKindUid: {dto.MetaObjectKindUid}");
                    transaction.Rollback();
                    return result;
                }

                var objectKindSettings = kind.ToSettings();
                var metaObjectProvider = new MetaObjectStorableProvider(_connection, objectKindSettings.Name);
                var metaObject = await metaObjectProvider.GetItemAsync(dto.MetaObjectUid, transaction);

                if (metaObject == null)
                {
                    result.Error(-1, $"{DictMain.CannotFindMetaObject}", $"MetaObjectUid: {dto.MetaObjectUid}");
                    transaction.Rollback();
                    return result;
                }

                var metaObjectSettings = metaObject.ToSettings();
                var dataTypesIndex = await _dataTypesService.GetIndexAsync(null);
                var allMetaObjects = new List<MetaObjectStorable>();

                foreach (var item in allKinds)
                {
                    metaObjectProvider = new MetaObjectStorableProvider(_connection, item.Name);
                    var metaObjects = await metaObjectProvider.GetCollectionAsync(transaction);

                    allMetaObjects.AddRange(metaObjects);
                }


                // Parse header.
                var parsedDto = DataObjectParser.Parse(dto.Item, metaObjectSettings, dataTypesIndex);
                var newObject = parsedDto.ToObject(metaObjectSettings);

                var provider = new DataObjectProvider(_connection, objectKindSettings, metaObjectSettings, dataTypesIndex);

                try
                {
                    var insertedUid = await provider.InsertAsync(newObject, transaction);
                    newObject.SetPrimaryKey(insertedUid);
                    foreach (var table in newObject.DetailTables)
                    {
                        var tableSettings = metaObjectSettings.DetailTables.FirstOrDefault(x => x.Uid == table.Uid);
                        if (tableSettings == null)
                        {
                            continue;
                        }
                        var tableProvider = new DataObjectDetailsTableProvider(_connection,
                            objectKindSettings,
                            metaObjectSettings,
                            tableSettings,
                            allKinds,
                            allMetaObjects,
                            dataTypesIndex);

                        await tableProvider.InsertAsync(insertedUid, table, transaction);
                    }

                    if (objectKindSettings.CanCreateRecords)
                    {
                        var recordsBuilder = new DataObjectsRecordsBuilder(_connection, 
                            transaction, 
                            objectKindSettings, 
                            metaObjectSettings, 
                            newObject, 
                            _kindProvider,
                            dataTypesIndex,
                            Common.Enums.EventTypeLevels.Trace);
                        var buildResult = await recordsBuilder.BuildAsync();
                    }

                    transaction.Commit();
                    result.Success(insertedUid, DictMain.ItemSaved);
                }
                catch (Exception ex)
                {
                    result.Error(-1, $"{DictMain.CannotCreateItem}", $"Message: {ex.Message}, Query: {provider.LastQuery}");
                }
            }
            return result;
        }

        public async Task<ResultWrapper<List<InMemoryLogMessage>>> UpdateAsync(DataObjectSaveDto dto)
        {
            var result = new ResultWrapper<List<InMemoryLogMessage>>();

            _connection.Open();
            using (IDbTransaction transaction = _connection.BeginTransaction())
            {

                var allKinds = await _kindProvider.GetCollectionAsync(transaction);
                var kind = allKinds.FirstOrDefault(x => x.Uid == dto.MetaObjectKindUid);

                if (kind == null)
                {
                    transaction.Rollback();
                    result.Error(-1, $"{DictMain.CannotFindMetaObjectKind}", $"MetaObjectKindUid: {dto.MetaObjectKindUid}");
                    return result;
                }
                var objectKindSettings = kind.ToSettings();

                var metaObjectProvider = new MetaObjectStorableProvider(_connection, objectKindSettings.Name);
                var metaObject = await metaObjectProvider.GetItemAsync(dto.MetaObjectUid, transaction);

                if (metaObject == null)
                {
                    result.Error(-1, $"{DictMain.CannotFindMetaObject}", $"MetaObjectUid: {dto.MetaObjectUid}");
                    return result;
                }

                var metaObjectSettings = metaObject.ToSettings();
                var dataTypesIndex = await _dataTypesService.GetIndexAsync(transaction);
                var allMetaObjects = new List<MetaObjectStorable>();

                foreach (var item in allKinds)
                {
                    metaObjectProvider = new MetaObjectStorableProvider(_connection, item.Name);
                    var metaObjects = await metaObjectProvider.GetCollectionAsync(transaction);

                    allMetaObjects.AddRange(metaObjects);
                }

                // Parse header.
                var parsedDto = DataObjectParser.Parse(dto.Item, metaObjectSettings, dataTypesIndex);
                var newItem = parsedDto.ToObject(metaObjectSettings);

                var provider = new DataObjectProvider(_connection, objectKindSettings, metaObjectSettings, dataTypesIndex);

                var uid = dto.Item.Header[metaObjectSettings.Header.PrimaryKey.Name];
                var objectUid = uid?.ToString() ?? string.Empty;
                var savedItem = await provider.GetItemAsync(objectUid, transaction);

                if (savedItem == null)
                {
                    transaction.Rollback();
                    result.Error(-1, $"Cannot find item: {uid}");
                    return result;
                }

                savedItem.CopyFrom(newItem);

                try
                {
                    var updateResult = await provider.UpdateAsync(savedItem, transaction);
                    foreach (var table in savedItem.DetailTables)
                    {
                        var tableSettings = metaObjectSettings.DetailTables.FirstOrDefault(x => x.Uid == table.Uid);
                        if (tableSettings == null)
                        {
                            continue;
                        }
                        var tableProvider = new DataObjectDetailsTableProvider(_connection,
                            objectKindSettings,
                            metaObjectSettings,
                            tableSettings,
                            allKinds,
                            allMetaObjects,
                            dataTypesIndex);

                        await tableProvider.UpdateAsync(objectUid, table, transaction);
                    }

                    var logMessages = new List<InMemoryLogMessage>();
                    if (objectKindSettings.CanCreateRecords)
                    {
                        var recordsBuilder = new DataObjectsRecordsBuilder(_connection,
                            transaction,
                            objectKindSettings,
                            metaObjectSettings,
                            savedItem,
                            _kindProvider,
                            dataTypesIndex,
                            Common.Enums.EventTypeLevels.Trace);
                        var buildResult = await recordsBuilder.BuildAsync();

                        logMessages.AddRange(recordsBuilder.Messages);
                    }

                    transaction.Commit();
                    result.Success(logMessages, DictMain.ItemSaved);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    result.Error(-1, $"{DictMain.CannotUpdateItem}", $"Message: {ex.Message}, Query: {provider.LastQuery}");
                }
            }

            return result;
        }

        public async Task<ResultWrapper<int>> DeleteItemAsync(string kindName, string objectName, string uid)
        {
            var result = new ResultWrapper<int>();

            _connection.Open();
            using (IDbTransaction transaction = _connection.BeginTransaction())
            {

                var allKinds = await _kindProvider.GetCollectionAsync(transaction);
                var kind = allKinds.FirstOrDefault(x => x.Name.Equals(kindName, StringComparison.InvariantCultureIgnoreCase));

                if (kind == null)
                {
                    transaction.Rollback();
                    result.Error(-1, $"{DictMain.CannotFindMetaObjectKind}: {kindName}");
                    return result;
                }

                var objectKindSettings = kind.ToSettings();


                var metaObjectProvider = new MetaObjectStorableProvider(_connection, objectKindSettings.Name);
                var metaObject = await metaObjectProvider.GetItemByNameAsync(objectName, transaction);

                if (metaObject == null)
                {
                    transaction.Rollback();
                    result.Error(-1, $"{DictMain.CannotFindMetaObject}: {kindName}.{objectName}");
                    return result;
                }

                var metaObjectSettings = metaObject.ToSettings();

                var allMetaObjects = new List<MetaObjectStorable>();

                foreach (var item in allKinds)
                {
                    metaObjectProvider = new MetaObjectStorableProvider(_connection, item.Name);
                    var metaObjects = await metaObjectProvider.GetCollectionAsync(transaction);

                    allMetaObjects.AddRange(metaObjects);
                }

                var dataTypesIndex = await _dataTypesService.GetIndexAsync(transaction);
                var provider = new DataObjectProvider(_connection, objectKindSettings, metaObjectSettings, dataTypesIndex);

                int deletedCount = 0;
                try
                {

                    if (objectKindSettings.CanCreateRecords)
                    {
                        var mockObject = new DataObject(metaObjectSettings, dataTypesIndex);
                        mockObject.SetPrimaryKey(uid);

                        var recordsBuilder = new DataObjectsRecordsBuilder(_connection,
                            transaction,
                            objectKindSettings,
                            metaObjectSettings,
                            mockObject,
                            _kindProvider,
                            dataTypesIndex,
                            Common.Enums.EventTypeLevels.Trace);
                        var buildResult = await recordsBuilder.DeleteAsync();
                    }

                    deletedCount = await provider.DeleteAsync(uid, transaction);
                    foreach (var tableSettings in metaObjectSettings.DetailTables)
                    {

                        var tableProvider = new DataObjectDetailsTableProvider(_connection,
                            objectKindSettings,
                            metaObjectSettings,
                            tableSettings,
                            allKinds,
                            allMetaObjects,
                            dataTypesIndex);

                        await tableProvider.DeleteTableAsync(uid, transaction);
                    }

                    transaction.Commit();
                }
                catch(Exception ex)
                {
                    transaction.Rollback();
                    result.Error(-1, $"Cannot delete item {kindName}.{objectName}:{uid}", ex.Message);
                    return result;
                }
             

                if (deletedCount > 0)
                {
                    result.Success(deletedCount, DictMain.ItemDeleted);
                }
                else if (deletedCount == 0)
                {
                    result.Error(-1, $"{DictMain.CannotFindItem} {kindName}.{objectName}:{uid}");
                }
            }
         

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
