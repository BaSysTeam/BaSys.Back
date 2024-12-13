using BaSys.Common.Enums;
using BaSys.Common.Infrastructure;
using BaSys.DAL.Models.App;
using BaSys.Host.DAL.DataProviders;
using BaSys.Logging.InMemory;
using BaSys.Metadata.Abstractions;
using BaSys.Metadata.Models;
using System.Data;

namespace BaSys.Core.Services.RecordsBuilder
{
    public sealed class DataObjectsRecordsBuilder
    {
        private readonly IDbConnection _connection;
        private readonly IDbTransaction _transaction;
        private readonly MetaObjectKindSettings _sourceKindSettings;
        private readonly MetaObjectStorableSettings _sourceSettings;
        private readonly DataObject _dataObject;
        private readonly IDataTypesIndex _dataTypesIndex;
        private readonly MetaObjectKindsProvider _kindsProvider;

        private readonly InMemoryLogger _logger;

        public IEnumerable<InMemoryLogMessage> Messages => _logger.Messages;

        public bool CreateRecords
        {
            get
            {
                var createRecordsColumnUid = _sourceKindSettings.RecordsSettings.SourceCreateRecordsColumnUid;
                var createRecordsColumn = _sourceSettings.Header.GetColumn(createRecordsColumnUid);
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
            _sourceKindSettings = kindSettings;
            _sourceSettings = settings;
            _dataObject = dataObject;

            _kindsProvider = kindsProvider;
            _dataTypesIndex = dataTypesIndex;

            _logger = new InMemoryLogger(logLevel);
        }

        public async Task<ResultWrapper<int>> BuildAsync()
        {
            var result = new ResultWrapper<int>();
            var totalRecordsCount = 0;

            if (!_sourceKindSettings.CanCreateRecords)
            {
                return result;
            }

            _logger.LogDebug($"Start records creating.");
            var destinationKindSettings = await _kindsProvider.GetSettingsAsync(_sourceKindSettings.RecordsSettings.StorageMetaObjectKindUid, _transaction);

            if (destinationKindSettings == null)
            {
                _logger.LogError("Cannot find MetaObjectKind by Uid {0}", _sourceKindSettings.RecordsSettings.StorageMetaObjectKindUid);
                return result;
            }

            var destinations = await GetDestinationSettingsAsync(destinationKindSettings.Name);
            var primaryKeyValue = _dataObject.GetPrimaryKey();
            var evaluator = new JintExpressionEvaluator(_logger);

            foreach (var recordsSettingnsItem in _sourceSettings.RecordsSettings)
            {
                destinations.TryGetValue(recordsSettingnsItem.DestinationMetaObjectUid, out var destinationSettings);
                if (destinationSettings == null)
                {
                    _logger.LogError("Cannot find MetaObject by Uid {0}", recordsSettingnsItem.DestinationMetaObjectUid);
                    continue;
                }
                _logger.LogDebug("Start processing {0}", destinationSettings.ToString());

                var requiredColumns = GetRequiredColumns(destinationSettings, _sourceKindSettings);

                if (!ValidateRequiredColumns(requiredColumns, _sourceKindSettings))
                {
                    result.Error(-1, $"Cannot create records.");
                    return result;
                }

                var provider = new DataObjectProvider(_connection, destinationKindSettings, destinationSettings, _dataTypesIndex);
                await provider.DeleteObjectRecordsAsync(requiredColumns.MetaObjectColumn.Name,
                    _sourceSettings.Uid,
                    requiredColumns.ObjectColumn.Name,
                    primaryKeyValue,
                    _transaction);

                if (!CreateRecords)
                {
                    // Only delete records. Not create new records.
                    continue;
                }

                var records = new List<DataObject>();
                var skippedCount = 0;

                foreach (var settingsRow in recordsSettingnsItem.Rows)
                {

                    var sourceTableSettings = _sourceSettings.Tables.FirstOrDefault(x => x.Uid == settingsRow.SourceUid);

                    if (sourceTableSettings == null)
                    {
                        var message = string.Format("Cannot find Source table by Uid {0}.", settingsRow.SourceUid);
                        _logger.LogError(message);
                        result.Error(-1, message);
                        return result;
                    }

                    if (sourceTableSettings.Name == "header")
                    {
                        if (!string.IsNullOrWhiteSpace(settingsRow.Condition))
                        {
                            // Evaluate condition
                            var conditionResult = EvaluateCondition(evaluator, settingsRow.Condition, _dataObject.Header, null);
                            if (!conditionResult)
                            {
                                _logger.LogDebug("Skip record Header->{0}", destinationSettings.Name);
                                skippedCount += 1;
                                continue;
                            }
                        }

                        // Create records by Header.
                        var record = CreateRecordByHeader(destinationSettings,
                                                          _sourceKindSettings,
                                                          _sourceSettings,
                                                          settingsRow,
                                                          requiredColumns,
                                                          _dataObject);

                        records.Add(record);

                    }
                    else
                    {
                        // Create records by DetailsTables.
                        var table = _dataObject.DetailTables.FirstOrDefault(x => x.Uid == sourceTableSettings.Uid);
                        if (table == null)
                        {
                            _logger.LogError("Cannot find table {0}.", sourceTableSettings.Name);
                        }
                        else
                        {
                            var rowNumber = 1;
                            foreach (var tableRow in table.Rows)
                            {
                                if (!string.IsNullOrWhiteSpace(settingsRow.Condition))
                                {
                                    // Evaluate condition
                                    var conditionResult = EvaluateCondition(evaluator, settingsRow.Condition, _dataObject.Header, tableRow);
                                    if (!conditionResult)
                                    {
                                        _logger.LogDebug("Skip record Table.{0}[1]->{2}", table.Name, rowNumber, destinationSettings.Name);
                                        skippedCount += 1;
                                        continue;
                                    }

                                }

                                var record = CreateRecordByTableRow(destinationSettings,
                                                                    _sourceKindSettings,
                                                                    _sourceSettings,
                                                                    settingsRow,
                                                                    requiredColumns,
                                                                    _dataObject,
                                                                    tableRow,
                                                                    rowNumber);

                                records.Add(record);

                                rowNumber++;



                            }
                        }
                    }

                }

                totalRecordsCount += records.Count;
                foreach (var record in records)
                {
                    //TODO: Implement Bulk insert.
                    await provider.InsertAsync(record, _transaction);
                }
                _logger.LogDebug($"{destinationSettings} {records.Count} records created. {skippedCount} records skipped.");

            }

            result.Success(totalRecordsCount);

            return result;
        }

        public async Task<ResultWrapper<int>> DeleteAsync()
        {
            var result = new ResultWrapper<int>();

            if (!_sourceKindSettings.CanCreateRecords)
            {
                return result;
            }

            var destinationKindSettings = await _kindsProvider.GetSettingsAsync(_sourceKindSettings.RecordsSettings.StorageMetaObjectKindUid, _transaction);

            if (destinationKindSettings == null)
            {
                _logger.LogError("Cannot find MetaObjectKind by Uid {0}", _sourceKindSettings.RecordsSettings.StorageMetaObjectKindUid);
                result.Error(-1, "Cannot delete records.");
                return result;
            }

            var destinations = await GetDestinationSettingsAsync(destinationKindSettings.Name);

            var totalDeleted = 0;
            foreach (var recordsSettingnsItem in _sourceSettings.RecordsSettings)
            {
                destinations.TryGetValue(recordsSettingnsItem.DestinationMetaObjectUid, out var destinationSettings);
                if (destinationSettings == null)
                {
                    _logger.LogError("Cannot find MetaObject by Uid {0}", recordsSettingnsItem.DestinationMetaObjectUid);
                    result.Error(-1, "Cannot delete records.");
                    return result;
                }

                var requiredColumns = GetRequiredColumns(destinationSettings, _sourceKindSettings);

                if (!ValidateRequiredColumns(requiredColumns, _sourceKindSettings))
                {
                    result.Error(-1, $"Cannot delete records.");
                    return result;
                }

                var primaryKeyValue = _dataObject.GetPrimaryKey();

                var provider = new DataObjectProvider(_connection, destinationKindSettings, destinationSettings, _dataTypesIndex);
                var deletedCount = await provider.DeleteObjectRecordsAsync(requiredColumns.MetaObjectColumn.Name,
                      _sourceSettings.Uid,
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

            foreach (var recordsSettingnsItem in _sourceSettings.RecordsSettings)
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
                _logger.LogError("Cannot find Row column by Uid {0}", _sourceKindSettings.RecordsSettings.StorageRowColumnUid);
            }

            if (requiredColumns.ObjectColumn == null)
            {
                _logger.LogError("Cannot find Object column by Uid {0}", _sourceKindSettings.RecordsSettings.StorageObjectColumnUid);
            }

            if (requiredColumns.MetaObjectColumn == null)
            {
                _logger.LogError("Cannot find MetaObject column by Uid {0}", _sourceKindSettings.RecordsSettings.StorageMetaObjectColumnUid);
            }

            if (requiredColumns.MetaObjectKindColumn == null)
            {
                _logger.LogError("Cannot find MetaObjectKind column by Uid {0}", _sourceKindSettings.RecordsSettings.StorageKindColumnUid);
            }

            return isValid;
        }

        private DataObject CreateNewRecord(MetaObjectStorableSettings destinationSettings,
            MetaObjectKindSettings sourceKindSettings,
            MetaObjectStorableSettings sourceSettings,
            RecordSettingsRequiredColumns requiredColumns,
            object? objectKey,
            int rowNumber)
        {

            var record = new DataObject(destinationSettings, _dataTypesIndex);
            record.SetValue(requiredColumns.RowColumn.Name, rowNumber);
            record.SetValue(requiredColumns.MetaObjectKindColumn.Name, sourceKindSettings.Uid);
            record.SetValue(requiredColumns.MetaObjectColumn.Name, sourceSettings.Uid);

            record.SetValue(requiredColumns.ObjectColumn.Name, objectKey);

            return record;

        }

        private DataObject CreateRecordByHeader(MetaObjectStorableSettings destinationSettings,
            MetaObjectKindSettings sourceKindSettings,
            MetaObjectStorableSettings sourceSettings,
            MetaObjectRecordsSettingsRow settingsRow,
            RecordSettingsRequiredColumns requiredColumns,
            DataObject dataObject)
        {
            var record = CreateNewRecord(destinationSettings,
                                         sourceKindSettings,
                                         sourceSettings,
                                         requiredColumns,
                                         dataObject.GetPrimaryKey(),
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
                        if (destinationColumn == null)
                        {
                            _logger.LogError("Cannot find column {0} in MetaObject {1}. Expression: {1}.",
                                settingsColumn.DestinationColumnUid,
                                destinationSettings.Name,
                                settingsColumn.Expression);
                        }
                        else
                        {
                            var currentValue = dataObject.GetValue<object>(parseResult.Name);
                            record.SetValue(destinationColumn.Name, currentValue);
                        }

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

            return record;
        }

        private DataObject CreateRecordByTableRow(MetaObjectStorableSettings destinationSettings,
            MetaObjectKindSettings sourceKindSettings,
            MetaObjectStorableSettings sourceSettings,
            MetaObjectRecordsSettingsRow settingsRow,
            RecordSettingsRequiredColumns requiredColumns,
            DataObject dataObject,
            DataObjectDetailsTableRow tableRow,
            int rowNumber)
        {
            var record = CreateNewRecord(destinationSettings,
                                         sourceKindSettings,
                                         sourceSettings,
                                         requiredColumns,
                                         dataObject.GetPrimaryKey(),
                                         rowNumber);

            var expressionParser = new RecordsExpressionParser();
            foreach (var settingsColumn in settingsRow.Columns)
            {
                var parseResult = expressionParser.Parse(settingsColumn.Expression);
                switch (parseResult.Kind)
                {
                    case RecordsExpressionKinds.Header:

                        var destinationColumn = destinationSettings.Header.GetColumn(settingsColumn.DestinationColumnUid);
                        if (destinationColumn == null)
                        {
                            _logger.LogError("Cannot find column {0} in {1}.{2}. Expression: {3}.",
                                                           settingsColumn.DestinationColumnUid,
                                                           sourceKindSettings.Name,
                                                           destinationSettings.Name,
                                                           settingsColumn.Expression);
                        }
                        else
                        {
                            var currentValue = _dataObject.GetValue<object>(parseResult.Name);
                            record.SetValue(destinationColumn.Name, currentValue);
                        }

                        break;
                    case RecordsExpressionKinds.Row:

                        var destinationColumnForRow = destinationSettings.Header.GetColumn(settingsColumn.DestinationColumnUid);
                        if (destinationColumnForRow == null)
                        {
                            _logger.LogError("Cannot find column {0} in {1}.{2}. Expression: {3}.",
                                                          settingsColumn.DestinationColumnUid,
                                                          sourceKindSettings.Name,
                                                          destinationSettings.Name,
                                                          settingsColumn.Expression);
                        }
                        else
                        {
                            var currentRowValue = tableRow.GetValue(parseResult.Name);
                            record.SetValue(destinationColumnForRow.Name, currentRowValue);
                        }

                        break;
                    case RecordsExpressionKinds.Error:
                        _logger.LogError("Error in expression {0}", settingsColumn.Expression);
                        break;
                    case RecordsExpressionKinds.Formula:
                        _logger.LogError("Cannot calculate formula {0}", settingsColumn.Expression);
                        break;
                }
            }

            return record;
        }

        private Dictionary<string, object?> BuildContext(Dictionary<string, object?> header, DataObjectDetailsTableRow row)
        {
            var context = new Dictionary<string, object?>();
            context.Add("header", header);
            if (row != null)
            {
                context.Add("row", row.Fields);
            }

            return context;
            
        }

        private bool EvaluateCondition(JintExpressionEvaluator evaluator, 
            string expression, 
            Dictionary<string, object?> header, 
            DataObjectDetailsTableRow row)
        {
            var result = false;
            var context = BuildContext(header, row);

            var expressionPrepared = expression.Replace("$h.", "context.header.", StringComparison.InvariantCultureIgnoreCase)
                .Replace("$r.", "context.row.", StringComparison.OrdinalIgnoreCase);

            evaluator.SetValue("context", context);
            result = evaluator.Evaluate<bool>(expressionPrepared);

            _logger.LogDebug("Condition {0} evaluated. Result {1}.", expression, result);

            return result;
        }

    }
}
