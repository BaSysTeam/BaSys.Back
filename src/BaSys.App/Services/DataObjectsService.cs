using BaSys.App.Abstractions;
using BaSys.Common.Infrastructure;
using BaSys.DAL.Models.App;
using BaSys.DTO.App;
using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.DataProviders;
using BaSys.Logging.Abstractions.Abstractions;
using BaSys.Metadata.Models;
using BaSys.Translation;
using System.Data;
using System.Security.AccessControl;


namespace BaSys.App.Services
{
    public sealed class DataObjectsService : IDataObjectsService, IDisposable
    {
        private readonly IDbConnection _connection;
        private readonly MetaObjectKindsProvider _kindProvider;
        private readonly ILoggerService _logger;
        private bool _disposed;

        public DataObjectsService(IMainConnectionFactory connectionFactory,
            ISystemObjectProviderFactory providerFactory,
            ILoggerService logger)
        {
            _connection = connectionFactory.CreateConnection();

            providerFactory.SetUp(_connection);
            _kindProvider = providerFactory.Create<MetaObjectKindsProvider>();

        }

        public async Task<ResultWrapper<DataObjectListDto>> GetCollectionAsync(string kindName, string objectName)
        {
            var result = new ResultWrapper<DataObjectListDto>();

            var objectKindSettings = await _kindProvider.GetSettingsByNameAsync(kindName);

            if (objectKindSettings == null)
            {
                result.Error(-1, $"{DictMain.CannotFindMetaObjectKind}: {kindName}");
                return result;
            }

            var metaObjectProvider = new MetaObjectStorableProvider(_connection, objectKindSettings.Name);
            var metaObject = await metaObjectProvider.GetItemByNameAsync(objectName, null);

            if (metaObject == null)
            {
                result.Error(-1, $"{DictMain.CannotFindMetaObject}: {kindName}.{objectName}");
                return result;
            }

            var metaObjectSettings = metaObject.ToSettings();
            var primitiveDataTypes = new PrimitiveDataTypes();
            var provider = new DataObjectProvider(_connection, objectKindSettings, metaObjectSettings, primitiveDataTypes);


            try
            {
                var collection = await provider.GetCollectionAsync(null);
                var listDto = new DataObjectListDto(objectKindSettings, metaObjectSettings, collection);

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

            var objectKindSettings = await _kindProvider.GetSettingsByNameAsync(kindName);

            if (objectKindSettings == null)
            {
                result.Error(-1, $"{DictMain.CannotFindMetaObjectKind}: {kindName}");
                return result;
            }

            var metaObjectProvider = new MetaObjectStorableProvider(_connection, objectKindSettings.Name);
            var metaObject = await metaObjectProvider.GetItemByNameAsync(objectName, null);

            if (metaObject == null)
            {
                result.Error(-1, $"{DictMain.CannotFindMetaObject}: {kindName}.{objectName}");
                return result;
            }

            var metaObjectSettings = metaObject.ToSettings();
            var primitiveDataTypes = new PrimitiveDataTypes();
            var provider = new DataObjectProvider(_connection, objectKindSettings, metaObjectSettings, primitiveDataTypes);


            try
            {
                var item = await provider.GetItemAsync(uid, null);
                if (item != null)
                {
                    var dto = new DataObjectWithMetadataDto(objectKindSettings, metaObjectSettings, item);
                    result.Success(dto);

                }
            }
            catch(Exception ex)
            {
                result.Error(-1, $"Cannot get item.", $"Message: {ex.Message}, query: {provider.LastQuery}");
            }

          

            return result;
        }

        public async Task<ResultWrapper<int>> InsertAsync(DataObjectSaveDto dto)
        {
            var result = new ResultWrapper<int>();

            var objectKindSettings = await _kindProvider.GetSettingsAsync(dto.MetaObjectKindUid);

            if (objectKindSettings == null)
            {
                result.Error(-1, $"{DictMain.CannotFindMetaObjectKind}", $"MetaObjectKindUid: {dto.MetaObjectKindUid}");
                return result;
            }

            var metaObjectProvider = new MetaObjectStorableProvider(_connection, objectKindSettings.Name);
            var metaObject = await metaObjectProvider.GetItemAsync(dto.MetaObjectUid, null);

            if (metaObject == null)
            {
                result.Error(-1, $"{DictMain.CannotFindMetaObject}", $"MetaObjectUid: {dto.MetaObjectUid}");
                return result;
            }


            var primitiveDataTypes = new PrimitiveDataTypes();
            var provider = new DataObjectProvider(_connection, objectKindSettings, metaObject.ToSettings(), primitiveDataTypes);

            var newObject = new DataObject(dto.Item.Header);

            try
            {
                var insertResult = await provider.InsertAsync(newObject, null);

                result.Success(insertResult);
            }
            catch (Exception ex)
            {
                result.Error(-1, $"{DictMain.CannotCreateItem}", $"Message: {ex.Message}, Query: {provider.LastQuery}");
            }

            return result;
        }

        public async Task<ResultWrapper<int>> UpdateAsync(DataObjectSaveDto dto)
        {
            var result = new ResultWrapper<int>();

            var objectKindSettings = await _kindProvider.GetSettingsAsync(dto.MetaObjectKindUid);

            if (objectKindSettings == null)
            {
                result.Error(-1, $"{DictMain.CannotFindMetaObjectKind}", $"MetaObjectKindUid: {dto.MetaObjectKindUid}");
                return result;
            }

            var metaObjectProvider = new MetaObjectStorableProvider(_connection, objectKindSettings.Name);
            var metaObject = await metaObjectProvider.GetItemAsync(dto.MetaObjectUid, null);

            if (metaObject == null)
            {
                result.Error(-1, $"{DictMain.CannotFindMetaObject}", $"MetaObjectUid: {dto.MetaObjectUid}");
                return result;
            }

            var metaObjectSettigs = metaObject.ToSettings();
            var primitiveDataTypes = new PrimitiveDataTypes();
            var provider = new DataObjectProvider(_connection, objectKindSettings, metaObjectSettigs, primitiveDataTypes);

            var newItem = new DataObject(dto.Item.Header);

            var uid = dto.Item.Header[metaObjectSettigs.Header.PrimaryKey.Name];
            var savedItem = await provider.GetItemAsync(uid?.ToString() ?? string.Empty, null);

            if (savedItem == null)
            {
                result.Error(-1, $"Cannot find item: {uid}");
                return result;  
            }

            savedItem.CopyFrom(newItem);

            try
            {
                var insertResult = await provider.UpdateAsync(savedItem, null);

                result.Success(insertResult);
            }
            catch (Exception ex)
            {
                result.Error(-1, $"{DictMain.CannotUpdateItem}", $"Message: {ex.Message}, Query: {provider.LastQuery}");
            }

            return result;
        }

        public async Task<ResultWrapper<int>> DeleteItemAsync(string kindName, string objectName, string uid)
        {
            var result = new ResultWrapper<int>();

            var objectKindSettings = await _kindProvider.GetSettingsByNameAsync(kindName);

            if (objectKindSettings == null)
            {
                result.Error(-1, $"{DictMain.CannotFindMetaObjectKind}: {kindName}");
                return result;
            }

            var metaObjectProvider = new MetaObjectStorableProvider(_connection, objectKindSettings.Name);
            var metaObject = await metaObjectProvider.GetItemByNameAsync(objectName, null);

            if (metaObject == null)
            {
                result.Error(-1, $"{DictMain.CannotFindMetaObject}: {kindName}.{objectName}");
                return result;
            }

            var metaObjectSettings = metaObject.ToSettings();

            var primitiveDataTypes = new PrimitiveDataTypes();
            var pkColumn = metaObjectSettings.Header.PrimaryKey;

            var pkDataType = primitiveDataTypes.GetDataType(pkColumn.DataTypeUid);

            var provider = new DataObjectProvider(_connection, objectKindSettings, metaObjectSettings, primitiveDataTypes);

            int deletedCount = -1;
            if (pkDataType.Equals(DataTypeDefaults.Int))
            {
                if (int.TryParse(uid, out var intValue))
                {
                    deletedCount = await provider.DeleteAsync<int>(intValue, null);
                }
                else
                {
                    result.Error(-1, $"Cannot parse value {uid} as int");
                }
            }
            else if (pkDataType.Equals(DataTypeDefaults.Long))
            {
                if (long.TryParse(uid, out var longValue))
                {
                    deletedCount = await provider.DeleteAsync<long>(longValue, null);
                }
                else
                {
                    result.Error(-1, $"Cannot parse value {uid} as int");
                }
            }
            else if (pkDataType.Equals(DataTypeDefaults.UniqueIdentifier))
            {
                if (Guid.TryParse(uid, out var guidValue))
                {
                    deletedCount = await provider.DeleteAsync<Guid>(guidValue, null);
                }
                else
                {
                    result.Error(-1, $"Cannot parse value {uid} as GUID");
                }
            }
            else if (pkDataType.Equals(DataTypeDefaults.String))
            {
                deletedCount = await provider.DeleteAsync<string>(uid, null);
            }
            else
            {
                result.Error(-1, $"Unsupported data type for primary key: {pkDataType}");
            }

            if (deletedCount > 0)
            {
                result.Success(deletedCount, DictMain.ItemDeleted);
            }
            else if (deletedCount == 0)
            {
                result.Error(-1, $"{DictMain.CannotFindItem} {kindName}.{objectName}:{uid}");
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
