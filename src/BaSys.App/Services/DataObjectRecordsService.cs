using BaSys.App.Abstractions;
using BaSys.App.Models.DataObjectRecordsDialog;
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

            var sourceKindSettings = GetSourceKindSettings(allKinds, kindName, result);
            if (sourceKindSettings == null) return result;
           
            var destinationKindSettings = GetDestinationKindSettings(allKinds, 
                sourceKindSettings.RecordsSettings.StorageMetaObjectKindUid, 
                result);
            if (destinationKindSettings == null) return result;

            var destinationSettings = await GetMetaObjectSettings(destinationKindSettings.Name, registerUid, result);
            if (destinationSettings == null) return result;
          
          
            var sourceSettings = await GetMetaObjectSettingsByName(kindName, objectName, result);
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
                objectUid, 
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
            ResultWrapper<DataTableDto> result )
        {
            var destinationKind = allKinds.FirstOrDefault(x=>x.Uid == destinationKindUid);
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
