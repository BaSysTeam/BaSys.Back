using BaSys.App.Abstractions;
using BaSys.Common.Infrastructure;
using BaSys.Core.Abstractions;
using BaSys.Core.Services;
using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.DataProviders;
using BaSys.Logging.Abstractions.Abstractions;
using BaSys.Metadata.Helpers;
using BaSys.Metadata.Models;
using BaSys.Translation;
using System.Data;
using System.Security.AccessControl;

namespace BaSys.App.Services
{
    public sealed class SelectItemsService : ISelectItemService
    {
        private readonly IDbConnection _connection;
        private readonly MetaObjectKindsProvider _kindProvider;
        private readonly IDataTypesService _dataTypesService;
        private readonly ILoggerService _logger;
        private bool _disposed;

        public SelectItemsService(IMainConnectionFactory connectionFactory,
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

        public async Task<ResultWrapper<IEnumerable<SelectItem>>> GetColllectionAsync(Guid dataTypeUid)
        {
            var result = new ResultWrapper<IEnumerable<SelectItem>>();

            // Get data type.
            var dataTypesIndex = await _dataTypesService.GetIndexAsync();
            if (!dataTypesIndex.IsDataType(dataTypeUid))
            {
                result.Error(-1, $"Data type not found: {dataTypeUid}");
                return result;
            }

            var dataType = dataTypesIndex.GetDataTypeSafe(dataTypeUid);

            if (!dataType.ObjectKindUid.HasValue)
            {
                result.Error(-1, $"Data type {dataTypeUid} is not object type.");
                return result;
            }

            // Get metaobject kind and metaobject.
            var metaObjectKind = await _kindProvider.GetItemAsync(dataType.ObjectKindUid.Value, null);

            if (metaObjectKind == null)
            {
                result.Error(-1, $"Metaobject kind not found: {dataType.ObjectKindUid.Value}");
                return result;
            }

            var metaObjectProvider = new MetaObjectStorableProvider(_connection, metaObjectKind.Name);
            var metaObject = await metaObjectProvider.GetItemAsync(dataTypeUid, null);


            if (metaObject == null)
            {
                result.Error(-1, $"{DictMain.CannotFindMetaObject}: {metaObjectKind.Name}.{dataTypeUid}");
                return result;
            }

            var objectKindSettings = metaObjectKind.ToSettings();
            var metaObjectSettings = metaObject.ToSettings();
            var selectItemsProvider = new SelectItemsProvider(_connection, objectKindSettings, metaObjectSettings, dataTypesIndex);

            var collection = await selectItemsProvider.GetCollectionAsync(null);

            result.Success(collection);

            return result;
        }

        public async Task<ResultWrapper<SelectItem>> GetItemAsync(Guid dataTypeUid, string uid)
        {
            var result = new ResultWrapper<SelectItem>();

            // Get data type.
            var dataTypesIndex = await _dataTypesService.GetIndexAsync();
            if (!dataTypesIndex.IsDataType(dataTypeUid))
            {
                result.Error(-1, $"Data type not found: {dataTypeUid}");
                return result;
            }

            var dataType = dataTypesIndex.GetDataTypeSafe(dataTypeUid);

            if (!dataType.ObjectKindUid.HasValue)
            {
                result.Error(-1, $"Data type {dataTypeUid} is not object type.");
                return result;
            }

            // Get metaobject kind and metaobject.
            var metaObjectKind = await _kindProvider.GetItemAsync(dataType.ObjectKindUid.Value, null);

            if (metaObjectKind == null)
            {
                result.Error(-1, $"Metaobject kind not found: {dataType.ObjectKindUid.Value}");
                return result;
            }

            var metaObjectProvider = new MetaObjectStorableProvider(_connection, metaObjectKind.Name);
            var metaObject = await metaObjectProvider.GetItemAsync(dataTypeUid, null);


            if (metaObject == null)
            {
                result.Error(-1, $"{DictMain.CannotFindMetaObject}: {metaObjectKind.Name}.{dataTypeUid}");
                return result;
            }

            var objectKindSettings = metaObjectKind.ToSettings();
            var metaObjectSettings = metaObject.ToSettings();
            var selectItemsProvider = new SelectItemsProvider(_connection, objectKindSettings, metaObjectSettings, dataTypesIndex);

            var selectItem = await selectItemsProvider.GetItemAsync(uid, null);

            if (selectItem == null) {
                result.Error(-1, $"{DictMain.CannotFindItem}. {metaObjectKind}.{metaObject}: {uid}");
            }
            else
            {
                result.Success(selectItem);
            }

            return result;
        }
    }
}
