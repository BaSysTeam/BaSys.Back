using BaSys.Common.Enums;
using BaSys.Common.Infrastructure;
using BaSys.Core.Services.RecordsBuilder;
using BaSys.DAL.Models.App;
using BaSys.Host.DAL.DataProviders;
using BaSys.Logging.InMemory;
using BaSys.Metadata.Abstractions;
using BaSys.Metadata.Helpers;
using BaSys.Metadata.Models;
using System.Data;

namespace BaSys.App.Services
{
    public sealed class DataObjectsRecordsBuilder
    {
        private readonly IDbConnection _connection;
        private readonly IDbTransaction _transaction;
        private readonly MetaObjectKindSettings _kindSettings;
        private readonly MetaObjectStorableSettings _settings;
        private readonly DataObject _dataObject;
        private readonly IDataTypesIndex _dataTypesIndex;
        private readonly MetaObjectKindsProvider _kindsProvider;

        private readonly InMemoryLogger _logger;

        public bool CreateRecords
        {
            get
            {
                var createRecordsColumnUid = _kindSettings.RecordsSettings.SourceCreateRecordsColumnUid;
                var createRecordsColumn = _settings.Header.GetColumn(createRecordsColumnUid);
                var createRecords = _dataObject.GetValue<bool>(createRecordsColumn?.Name ?? "");

                return createRecords;
            }
        }

        public DataObjectsRecordsBuilder(IDbConnection connection,
            IDbTransaction transaction,
            MetaObjectKindSettings kindSettings,
            MetaObjectStorableSettings settings,
            DataObject dataObject,
            MetaObjectKindsProvider kindsProvider,
            IDataTypesIndex dataTypesIndex,
            EventTypeLevels logLevel)
        {
            _connection = connection;
            _transaction = transaction;
            _kindSettings = kindSettings;
            _settings = settings;
            _dataObject = dataObject;

            _kindsProvider = kindsProvider;
            _dataTypesIndex = dataTypesIndex;

            _logger = new InMemoryLogger(logLevel);
        }

        public async Task<ResultWrapper<int>> BuildAsync()
        {
            var result = new ResultWrapper<int>();

            if (!_kindSettings.CanCreateRecords)
            {
                return result;
            }

            var destinationKindSettings = await _kindsProvider.GetSettingsAsync(_kindSettings.RecordsSettings.StorageMetaObjectKindUid, _transaction);

            if (destinationKindSettings == null)
            {
                _logger.LogError("Cannot find MetaObjectKind by Uid {0}", _kindSettings.RecordsSettings.StorageMetaObjectKindUid);
                return result;
            }

            var destinations = await GetDestinationSettingsAsync(destinationKindSettings.Name);
            var primaryKeyValue = _dataObject.GetPrimaryKey();

            foreach (var recordsSettingnsItem in _settings.RecordsSettings)
            {
                destinations.TryGetValue(recordsSettingnsItem.DestinationMetaObjectUid, out var destinationSettings);
                if (destinationSettings == null)
                {
                    _logger.LogError("Cannot find MetaObject by Uid {0}", recordsSettingnsItem.DestinationMetaObjectUid);
                    continue;
                }

                var requiredColumns = GetRequiredColumns(destinationSettings, _kindSettings);

                if (!ValidateRequiredColumns(requiredColumns, _kindSettings))
                {
                    result.Error(-1, $"Cannot create records.");
                    return result;
                }

                var provider = new DataObjectProvider(_connection, destinationKindSettings, destinationSettings, _dataTypesIndex);
                await provider.DeleteObjectRecordsAsync(requiredColumns.MetaObjectColumn.Name,
                    destinationSettings.Uid,
                    requiredColumns.ObjectColumn.Name,
                    primaryKeyValue,
                    _transaction);

                if (!CreateRecords)
                {
                    // Only delete records. Not create new records.
                    continue;
                }

                foreach (var settingsRow in recordsSettingnsItem.Rows)
                {

                    var sourceTableSettings = _settings.Tables.FirstOrDefault(x => x.Uid == settingsRow.SourceUid);

                    if (sourceTableSettings == null)
                    {
                        var message = string.Format("Cannot find Source table by Uid {0}", settingsRow.SourceUid);
                        _logger.LogError(message);
                        result.Error(-1, message);
                        return result;
                    }

                    if (sourceTableSettings.Name == "header")
                    {

                        var record = CreateNewRecord(destinationKindSettings, 
                            destinationSettings, 
                            requiredColumns, 
                            primaryKeyValue, 
                            1);

                        // Buid records by header.
                        var expressionParser = new RecordsExpressionParser();
                        foreach (var settingsColumn in settingsRow.Columns)
                        {
                            var parseResult = expressionParser.Parse(settingsColumn.Expression);
                            switch (parseResult.Kind)
                            {
                                case RecordsExpressionKinds.Header:

                                    var destinationColumn = destinationSettings.Header.GetColumn(settingsColumn.DestinationColumnUid);
                                    var currentValue = _dataObject.GetValue<object>(parseResult.Name);
                                    record.SetValue(destinationColumn.Name, currentValue);

                                    break;
                                case RecordsExpressionKinds.Row:
                                    _logger.LogError("Cannot calculate row expression {0} for Header source.", settingsColumn.Expression);
                                    break;
                                case RecordsExpressionKinds.Error:
                                    _logger.LogError("Error in expression {0}", settingsColumn.Expression);
                                    break;
                                case RecordsExpressionKinds.Formula:
                                    _logger.LogError("Cannot calculate formula {0}", settingsColumn.Expression);
                                    break;
                            }
                        }

                        await provider.InsertAsync(record, _transaction);

                    }
                    else
                    {
                        // Create records from DetailsTables.
                        var table = _dataObject.DetailTables.FirstOrDefault(x => x.Uid == sourceTableSettings.Uid);
                        if (table == null)
                        {
                            _logger.LogError("Cannot find table {0}", sourceTableSettings.Name);
                        }
                        else
                        {
                            var rowNumber = 1;
                            foreach (var tableRow in table.Rows)
                            {
                               
                                var record = CreateNewRecord(destinationKindSettings, 
                                    destinationSettings, 
                                    requiredColumns, 
                                    primaryKeyValue, 
                                    rowNumber);

                                var expressionParser = new RecordsExpressionParser();
                                foreach (var settingsColumn in settingsRow.Columns)
                                {
                                    var parseResult = expressionParser.Parse(settingsColumn.Expression);
                                    switch (parseResult.Kind)
                                    {
                                        case RecordsExpressionKinds.Header:

                                            var destinationColumn = destinationSettings.Header.GetColumn(settingsColumn.DestinationColumnUid);
                                            var currentValue = _dataObject.GetValue<object>(parseResult.Name);
                                            record.SetValue(destinationColumn.Name, currentValue);

                                            break;
                                        case RecordsExpressionKinds.Row:
                                            var destinationTableColumn = destinationSettings.Header.GetColumn(settingsColumn.DestinationColumnUid);
                                            var currentRowValue = tableRow.GetValue(parseResult.Name);
                                            record.SetValue(destinationTableColumn.Name, currentRowValue);

                                            break;
                                        case RecordsExpressionKinds.Error:
                                            _logger.LogError("Error in expression {0}", settingsColumn.Expression);
                                            break;
                                        case RecordsExpressionKinds.Formula:
                                            _logger.LogError("Cannot calculate formula {0}", settingsColumn.Expression);
                                            break;
                                    }
                                }
                                rowNumber++;

                                await provider.InsertAsync(record, _transaction);

                            }
                        }
                    }

                }
            }

            return result;
        }

        public async Task<ResultWrapper<int>> DeleteAsync()
        {
            var result = new ResultWrapper<int>();

            if (!_kindSettings.CanCreateRecords)
            {
                return result;
            }

            var destinationKindSettings = await _kindsProvider.GetSettingsAsync(_kindSettings.RecordsSettings.StorageMetaObjectKindUid, _transaction);

            if (destinationKindSettings == null)
            {
                _logger.LogError("Cannot find MetaObjectKind by Uid {0}", _kindSettings.RecordsSettings.StorageMetaObjectKindUid);
                result.Error(-1, "Cannot delete records.");
                return result;
            }

            var destinations = await GetDestinationSettingsAsync(destinationKindSettings.Name);

            var totalDeleted = 0;
            foreach (var recordsSettingnsItem in _settings.RecordsSettings)
            {
                destinations.TryGetValue(recordsSettingnsItem.DestinationMetaObjectUid, out var destinationSettings);
                if (destinationSettings == null)
                {
                    _logger.LogError("Cannot find MetaObject by Uid {0}", recordsSettingnsItem.DestinationMetaObjectUid);
                    result.Error(-1, "Cannot delete records.");
                    return result;
                }

                var requiredColumns = GetRequiredColumns(destinationSettings, _kindSettings);

                if (!ValidateRequiredColumns(requiredColumns, _kindSettings))
                {
                    result.Error(-1, $"Cannot delete records.");
                    return result;
                }

                var primaryKeyValue = _dataObject.GetPrimaryKey();

                var provider = new DataObjectProvider(_connection, destinationKindSettings, destinationSettings, _dataTypesIndex);
                var deletedCount = await provider.DeleteObjectRecordsAsync(requiredColumns.MetaObjectColumn.Name,
                      destinationSettings.Uid,
                      requiredColumns.ObjectColumn.Name,
                      primaryKeyValue,
                      _transaction);

                totalDeleted += deletedCount;
            }

            result.Success(totalDeleted, $"Deleted {totalDeleted} records");
            return result;
        }

        private async Task<Dictionary<Guid, MetaObjectStorableSettings>> GetDestinationSettingsAsync(string destinationKindName)
        {
            var metaObjectDestinationProvider = new MetaObjectStorableProvider(_connection, destinationKindName);
            var destinations = new Dictionary<Guid, MetaObjectStorableSettings>();

            foreach (var recordsSettingnsItem in _settings.RecordsSettings)
            {
                var metaObjectSettings = await metaObjectDestinationProvider.GetSettingsItemAsync(recordsSettingnsItem.DestinationMetaObjectUid, _transaction);

                if (metaObjectSettings == null)
                {
                    continue;
                }

                destinations.Add(recordsSettingnsItem.DestinationMetaObjectUid, metaObjectSettings);
            }

            return destinations;
        }

        private RecordSettingsRequiredColumns GetRequiredColumns(MetaObjectStorableSettings destinationSettings,
            MetaObjectKindSettings creatorKindSettings)
        {
            var requiredColumns = new RecordSettingsRequiredColumns();

            requiredColumns.RowColumn = destinationSettings.Header.GetColumn(creatorKindSettings.RecordsSettings.StorageRowColumnUid);
            requiredColumns.ObjectColumn = destinationSettings.Header.GetColumn(creatorKindSettings.RecordsSettings.StorageObjectColumnUid);
            requiredColumns.MetaObjectColumn = destinationSettings.Header.GetColumn(creatorKindSettings.RecordsSettings.StorageMetaObjectColumnUid);
            requiredColumns.MetaObjectKindColumn = destinationSettings.Header.GetColumn(creatorKindSettings.RecordsSettings.StorageKindColumnUid);

            return requiredColumns;
        }

        private bool ValidateRequiredColumns(RecordSettingsRequiredColumns requiredColumns, MetaObjectKindSettings creatorKindSettings)
        {
            var isValid = true;
            if (requiredColumns.RowColumn == null)
            {
                _logger.LogError("Cannot find Row column by Uid {0}", _kindSettings.RecordsSettings.StorageRowColumnUid);
            }

            if (requiredColumns.ObjectColumn == null)
            {
                _logger.LogError("Cannot find Object column by Uid {0}", _kindSettings.RecordsSettings.StorageObjectColumnUid);
            }

            if (requiredColumns.MetaObjectColumn == null)
            {
                _logger.LogError("Cannot find MetaObject column by Uid {0}", _kindSettings.RecordsSettings.StorageMetaObjectColumnUid);
            }

            if (requiredColumns.MetaObjectKindColumn == null)
            {
                _logger.LogError("Cannot find MetaObjectKind column by Uid {0}", _kindSettings.RecordsSettings.StorageKindColumnUid);
            }

            return isValid;
        }

        private DataObject CreateNewRecord(MetaObjectKindSettings destinationKindSettings, 
            MetaObjectStorableSettings destinationSettings, 
            RecordSettingsRequiredColumns requiredColumns, 
            object objectKey, 
            int rowNumber)
        {

            var record = new DataObject(destinationSettings, _dataTypesIndex);
            record.SetValue(requiredColumns.RowColumn.Name, rowNumber);
            record.SetValue(requiredColumns.MetaObjectKindColumn.Name, destinationKindSettings.Uid);
            record.SetValue(requiredColumns.MetaObjectColumn.Name, destinationSettings.Uid);

            record.SetValue(requiredColumns.ObjectColumn.Name, objectKey);

            return record;

        }


    }
}
