using BaSys.App.Abstractions;
using BaSys.App.Models.DataObjectRecordsDialog;
using BaSys.Common.Infrastructure;
using BaSys.Core.Abstractions;
using BaSys.Core.Services;
using BaSys.DTO.Core;
using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.DataProviders;
using BaSys.Translation;
using System.Data;

namespace BaSys.App.Features.DataObjectRecords.Queries
{
    public sealed class GetRecordsDialogModelQueryHandler: IGetRecordsDialogModelQueryHandler, IDisposable
    {
        private readonly IDbConnection _connection;
        private readonly ISystemObjectProviderFactory _providerFactory;
        private readonly MetaObjectKindsProvider _kindProvider;
        private readonly IDataTypesService _dataTypesService;
        private bool _disposed;

        public GetRecordsDialogModelQueryHandler(IMainConnectionFactory connectionFactory,
            ISystemObjectProviderFactory providerFactory)
        {
            _connection = connectionFactory.CreateConnection();
            _providerFactory = providerFactory;
            _providerFactory.SetUp(_connection);

            _kindProvider = _providerFactory.Create<MetaObjectKindsProvider>();

            _dataTypesService = new DataTypesService(providerFactory);
            _dataTypesService.SetUp(_connection);
        }

        public async Task<ResultWrapper<DataObjectRecordsDialogViewModel>> ExecuteAsync(GetRecordsDialogModelQuery query)
        {
            var result = new ResultWrapper<DataObjectRecordsDialogViewModel>();

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

        private async Task<ResultWrapper<DataObjectRecordsDialogViewModel>> ExecuteQueryAsync(GetRecordsDialogModelQuery query)
        {
            var result = new ResultWrapper<DataObjectRecordsDialogViewModel>();

            var allKinds = await _kindProvider.GetCollectionAsync(null);
            var kind = allKinds.FirstOrDefault(x => x.Name.Equals(query.KindName, StringComparison.InvariantCultureIgnoreCase));

            if (kind == null)
            {
                result.Error(-1, $"{DictMain.CannotFindMetaObjectKind}: {query.KindName}");
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
            var metaObject = await metaObjectProvider.GetItemByNameAsync(query.ObjectName, null);

            if (metaObject == null)
            {
                result.Error(-1, $"{DictMain.CannotFindMetaObject}: {query.KindName}.{query.ObjectName}");
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
                    if (destinationColumn.DataSettings.PrimaryKey)
                    {
                        continue;
                    }

                    if (excludedColumns.Contains(destinationColumn.Uid))
                    {
                        continue;
                    }

                    var basSysDataType = dataTypesIndex.GetDataTypeSafe(destinationColumn.DataSettings.DataTypeUid);

                    var column = new DataTableColumnDto()
                    {
                        Uid = destinationColumn.Uid.ToString(),
                        Name = destinationColumn.Name,
                        Title = destinationColumn.Title,
                        DataType = DataTableColumnDto.ConvertType(basSysDataType.DbType),
                        NumberDigits = destinationColumn.DataSettings.NumberDigits,
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
