using BaSys.App.Abstractions;
using BaSys.App.Models.DataObjectRecordsDialog;
using BaSys.Common.Infrastructure;
using BaSys.Core.Abstractions;
using BaSys.Core.Services;
using BaSys.DTO.Core;
using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.DataProviders;
using BaSys.Metadata.Models;
using BaSys.Translation;
using System.Data;
using System.Security.AccessControl;

namespace BaSys.App.Services
{
    public sealed class DataObjectRecordsService : IDataObjectRecordsService, IDisposable
    {
        private readonly IDbConnection _connection;
        private readonly ISystemObjectProviderFactory _providerFactory;
        private readonly MetaObjectKindsProvider _kindProvider;
        private readonly IDataTypesService _dataTypesService;
        private bool _disposed;

        public DataObjectRecordsService(IMainConnectionFactory connectionFactory,
            ISystemObjectProviderFactory providerFactory)
        {
            _connection = connectionFactory.CreateConnection();
            _providerFactory = providerFactory;
            _providerFactory.SetUp(_connection);

            _kindProvider = _providerFactory.Create<MetaObjectKindsProvider>();

            _dataTypesService = new DataTypesService(providerFactory);
            _dataTypesService.SetUp(_connection);
        }

        public async Task<ResultWrapper<DataObjectRecordsDialogViewModel>> GetModelAsync(string kind, string name, string uid)
        {
            var result = new ResultWrapper<DataObjectRecordsDialogViewModel>();

            try
            {
                result = await ExecuteGetModelAsync(kind, name, uid);
            }
            catch (Exception ex)
            {
                result.Error(-1, $"Cannot get model. Message: {ex.Message}.", ex.StackTrace);
            }

            return result;
        }

        private async Task<ResultWrapper<DataObjectRecordsDialogViewModel>> ExecuteGetModelAsync(string kindName, string objectName, string uid)
        {
            var result = new ResultWrapper<DataObjectRecordsDialogViewModel>();

            var allKinds = await _kindProvider.GetCollectionAsync(null);
            var kind = allKinds.FirstOrDefault(x => x.Name.Equals(kindName, StringComparison.InvariantCultureIgnoreCase));

            if (kind == null)
            {
                result.Error(-1, $"{DictMain.CannotFindMetaObjectKind}: {kindName}");
                return result;
            }

            var objectKindSettings = kind.ToSettings();
            var excludedColumns = new List<Guid>
            {
                objectKindSettings.RecordsSettings.StorageKindColumnUid,
                objectKindSettings.RecordsSettings.StorageMetaObjectColumnUid,
                objectKindSettings.RecordsSettings.StorageKindColumnUid,
                objectKindSettings.RecordsSettings.StorageObjectColumnUid
            };

            var recordsDestinationKind = allKinds.FirstOrDefault(x => x.Uid == objectKindSettings.RecordsSettings.StorageMetaObjectKindUid);

            if (recordsDestinationKind == null)
            {
                result.Error(-1, $"Cannot find records meta object kind by uid: {objectKindSettings.RecordsSettings.StorageMetaObjectKindUid}");
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

            var recordsMetaObjectProvider = new MetaObjectStorableProvider(_connection, recordsDestinationKind.Name);


            var model = new DataObjectRecordsDialogViewModel();
            var dataTypesIndex = await _dataTypesService.GetIndexAsync(null);

            foreach (var recordsSettingsItem in metaObjectSettings.RecordsSettings)
            {
                var destinationMetaObject = await recordsMetaObjectProvider.GetItemAsync(recordsSettingsItem.DestinationMetaObjectUid, null);

                if (destinationMetaObject == null)
                {
                    continue;
                }

                var tab = new DataObjectRecordsDialogTab()
                {
                    Uid = destinationMetaObject.Uid,
                    Title = destinationMetaObject.Title
                };

                var destinationSettings = destinationMetaObject.ToSettings();

                foreach (var destinationColumn in destinationSettings.Header.Columns)
                {
                    if (destinationColumn.PrimaryKey)
                    {
                        continue;
                    }

                    if (excludedColumns.Contains(destinationColumn.Uid))
                    {
                        continue;
                    }

                    var basSysDataType = dataTypesIndex.GetDataTypeSafe(destinationColumn.DataTypeUid);

                    var column = new DataTableColumnDto()
                    {
                        Uid = destinationColumn.Uid.ToString(),
                        Name = destinationColumn.Name,
                        Title = destinationColumn.Title,
                        DataType = DataTableColumnDto.ConvertType(basSysDataType.DbType),
                        NumberDigits = destinationColumn.NumberDigits,
                        IsReference = !basSysDataType.IsPrimitive
                    };

                    if (destinationColumn.Uid == objectKindSettings.RecordsSettings.StoragePeriodColumnUid)
                    {
                        column.Width = "160px";
                    }

                    if (destinationColumn.Uid == objectKindSettings.RecordsSettings.StorageRowColumnUid)
                    {
                        column.Width = "70px";
                    }

                    if (column.IsReference)
                    {
                        column.Name = $"{column.Name}_display";
                    }

                    tab.Columns.Add(column);

                }

                model.Tabs.Add(tab);
            }

            result.Success(model);

            return result;
        }

        public async Task<ResultWrapper<DataTableDto>> GetRecordsAsync(string kind,
            string name,
            string objectUid,
            Guid registerUid)
        {
            var result = new ResultWrapper<DataTableDto>();

            try
            {
                result = await ExecuteGetRecordsAsync(kind, name, objectUid, registerUid);
            }
            catch (Exception ex)
            {
                result.Error(-1, $"Cannot get records. Message: {ex.Message}.", ex.StackTrace);
            }

            return result;
        }

        private async Task<ResultWrapper<DataTableDto>> ExecuteGetRecordsAsync(string kindName,
            string objectName,
            string objectUid,
            Guid registerUid)
        {
            var result = new ResultWrapper<DataTableDto>();

            var allKinds = await _kindProvider.GetCollectionAsync(null);
            var kind = allKinds.FirstOrDefault(x => x.Name.Equals(kindName, StringComparison.InvariantCultureIgnoreCase));

            if (kind == null)
            {
                result.Error(-1, $"{DictMain.CannotFindMetaObjectKind}: {kindName}");
                return result;
            }

            var objectKindSettings = kind.ToSettings();

            var registerMetaObjectKind = allKinds.FirstOrDefault(x => x.Uid == objectKindSettings.RecordsSettings.StorageMetaObjectKindUid);

            if (registerMetaObjectKind == null)
            {
                result.Error(-1, $"Cannot find records meta object kind by uid: {objectKindSettings.RecordsSettings.StorageMetaObjectKindUid}");
                return result;
            }
            
            var metaObjectRegisterProvider = new MetaObjectStorableProvider(_connection, registerMetaObjectKind.Name);
            var metaObjectRegister = await metaObjectRegisterProvider.GetItemAsync(registerUid, null);

            if (metaObjectRegister == null)
            {
                result.Error(-1, $"{DictMain.CannotFindMetaObject}: {registerMetaObjectKind.Name}.{registerUid}");
                return result;
            }

            var metaObjectOperationProvider = new MetaObjectStorableProvider(_connection, objectKindSettings.Name);
            var metaObjectOperation = await metaObjectOperationProvider.GetItemByNameAsync(objectName, null);

            if (metaObjectRegister == null)
            {
                result.Error(-1, $"{DictMain.CannotFindMetaObject}: {objectKindSettings.Name}.{objectName}");
                return result;
            }

            var registerKindSettings = registerMetaObjectKind.ToSettings();
            var registerSettings = metaObjectRegister.ToSettings();
            var dataTypesIndex = await _dataTypesService.GetIndexAsync(null);

            var allMetaObjects = new List<MetaObjectStorable>();

            foreach (var item in allKinds)
            {
                var metaObjectProvider = new MetaObjectStorableProvider(_connection, item.Name);
                var metaObjects = await metaObjectProvider.GetCollectionAsync(null);

                allMetaObjects.AddRange(metaObjects);
            }

            var metaObjectColumn = registerSettings.Header.GetColumn(objectKindSettings.RecordsSettings.StorageMetaObjectColumnUid);
            var objectColumn = registerSettings.Header.GetColumn(objectKindSettings.RecordsSettings.StorageObjectColumnUid);

            var provider = new DataObjectListProvider(_connection, 
                registerMetaObjectKind.ToSettings(), 
                registerSettings, 
                allKinds, 
                allMetaObjects, 
                dataTypesIndex);

            var collection = await provider.GetObjectRecordsWithDisplaysAsync(metaObjectColumn.Name, 
                metaObjectOperation.Uid, 
                objectColumn.Name, 
                objectUid, 
                null);

            var dataTable = new DataTableDto();
            foreach(var dataObject in collection)
            {
                dataTable.AddRow(dataObject.Header);
            }
            result.Success(dataTable);

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
