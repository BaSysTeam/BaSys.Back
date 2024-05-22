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
    public sealed class DataObjectsService: IDataObjectsService, IDisposable
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

            if(objectKindSettings == null)
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

        public async Task<ResultWrapper<int>> InsertAsync(DataObjectDto dto)
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
                result.Error(-1, $"{DictMain.CannotFindMetaObject}",$"MetaObjectUid: {dto.MetaObjectUid}");
                return result;
            }


            var primitiveDataTypes = new PrimitiveDataTypes();
            var provider = new DataObjectProvider(_connection, objectKindSettings, metaObject.ToSettings(), primitiveDataTypes);

            var newObject = new DataObject(dto.Header);

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
