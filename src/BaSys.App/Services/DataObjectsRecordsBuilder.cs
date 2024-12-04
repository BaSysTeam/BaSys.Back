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

            if (!CreateRecords)
            {
                return result;
            }


            var destinationKindSettings = await _kindsProvider.GetSettingsAsync(_kindSettings.RecordsSettings.StorageMetaObjectKindUid, _transaction);

            if (destinationKindSettings == null)
            {
                _logger.LogError("Cannot find MetaObjectKind by Uid {0}", _kindSettings.RecordsSettings.StorageMetaObjectKindUid);
                return result;
            }

            var metaObjectDestinationProvider = new MetaObjectStorableProvider(_connection, destinationKindSettings.Name);
            var destinations = new Dictionary<Guid, MetaObjectStorableSettings>();

            foreach (var recordsSettingnsItem in _settings.RecordsSettings)
            {
                var metaObjectSettings = await metaObjectDestinationProvider.GetSettingsItemAsync(recordsSettingnsItem.DestinationMetaObjectUid, _transaction);

                if (metaObjectSettings == null)
                {
                    _logger.LogError("Cannot find MetaObject by Uid {0}", recordsSettingnsItem.DestinationMetaObjectUid);
                    return result;
                }

                destinations.Add(recordsSettingnsItem.DestinationMetaObjectUid, metaObjectSettings);
            }


            foreach (var recordsSettingnsItem in _settings.RecordsSettings)
            {
                var destinationSettings = destinations[recordsSettingnsItem.DestinationMetaObjectUid];

                var rowColumn = destinationSettings.Header.GetColumn(_kindSettings.RecordsSettings.StorageRowColumnUid);
                var objectColumn = destinationSettings.Header.GetColumn(_kindSettings.RecordsSettings.StorageObjectColumnUid);
                var metaObjectColumn = destinationSettings.Header.GetColumn(_kindSettings.RecordsSettings.StorageMetaObjectColumnUid);
                var metaObjectKindColumn = destinationSettings.Header.GetColumn(_kindSettings.RecordsSettings.StorageKindColumnUid);

                if (rowColumn == null)
                {
                    var message = string.Format("Cannot find Row column by Uid {0}", _kindSettings.RecordsSettings.StorageRowColumnUid);
                    _logger.LogError(message);
                    result.Error(-1, message);
                    return result;
                }

                if (objectColumn == null)
                {
                    var message = string.Format("Cannot find Object column by Uid {0}", _kindSettings.RecordsSettings.StorageObjectColumnUid);
                    _logger.LogError(message);
                    result.Error(-1, message);
                    return result;
                }

                if (metaObjectColumn == null)
                {
                    var message = string.Format("Cannot find MetaObject column by Uid {0}", _kindSettings.RecordsSettings.StorageMetaObjectColumnUid);
                    _logger.LogError(message);
                    result.Error(-1, message);
                    return result;
                }

                if (metaObjectKindColumn == null)
                {
                    var message = string.Format("Cannot find MetaObjectKind column by Uid {0}", _kindSettings.RecordsSettings.StorageKindColumnUid);
                    _logger.LogError(message);
                    result.Error(-1, message);
                    return result;
                }

                var primaryKeyValue = _dataObject.GetPrimaryKey();

                var provider = new DataObjectProvider(_connection, destinationKindSettings, destinationSettings, _dataTypesIndex);
                await provider.DeleteObjectRecordsAsync(metaObjectColumn.Name,
                    destinationSettings.Uid,
                    objectColumn.Name,
                    primaryKeyValue,
                    _transaction);

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
                        var record = new DataObject(destinationSettings, _dataTypesIndex);
                        record.SetValue(rowColumn.Name, 1);
                        record.SetValue(metaObjectKindColumn.Name, destinationKindSettings.Uid);
                        record.SetValue(metaObjectColumn.Name, destinationSettings.Uid);

                        record.SetValue(objectColumn.Name, primaryKeyValue);

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
                                var record = new DataObject(destinationSettings, _dataTypesIndex);
                                record.SetValue(rowColumn.Name, rowNumber);
                                record.SetValue(metaObjectKindColumn.Name, destinationKindSettings.Uid);
                                record.SetValue(metaObjectColumn.Name, destinationSettings.Uid);

                                record.SetValue(objectColumn.Name, primaryKeyValue);

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
                return result;
            }

            var metaObjectDestinationProvider = new MetaObjectStorableProvider(_connection, destinationKindSettings.Name);
            var destinations = new Dictionary<Guid, MetaObjectStorableSettings>();

            foreach (var recordsSettingnsItem in _settings.RecordsSettings)
            {
                var metaObjectSettings = await metaObjectDestinationProvider.GetSettingsItemAsync(recordsSettingnsItem.DestinationMetaObjectUid, _transaction);

                if (metaObjectSettings == null)
                {
                    _logger.LogError("Cannot find MetaObject by Uid {0}", recordsSettingnsItem.DestinationMetaObjectUid);
                    return result;
                }

                destinations.Add(recordsSettingnsItem.DestinationMetaObjectUid, metaObjectSettings);
            }

            foreach (var recordsSettingnsItem in _settings.RecordsSettings)
            {
                var destinationSettings = destinations[recordsSettingnsItem.DestinationMetaObjectUid];

                var rowColumn = destinationSettings.Header.GetColumn(_kindSettings.RecordsSettings.StorageRowColumnUid);
                var objectColumn = destinationSettings.Header.GetColumn(_kindSettings.RecordsSettings.StorageObjectColumnUid);
                var metaObjectColumn = destinationSettings.Header.GetColumn(_kindSettings.RecordsSettings.StorageMetaObjectColumnUid);
                var metaObjectKindColumn = destinationSettings.Header.GetColumn(_kindSettings.RecordsSettings.StorageKindColumnUid);

                if (rowColumn == null)
                {
                    var message = string.Format("Cannot find Row column by Uid {0}", _kindSettings.RecordsSettings.StorageRowColumnUid);
                    _logger.LogError(message);
                    result.Error(-1, message);
                    return result;
                }

                if (objectColumn == null)
                {
                    var message = string.Format("Cannot find Object column by Uid {0}", _kindSettings.RecordsSettings.StorageObjectColumnUid);
                    _logger.LogError(message);
                    result.Error(-1, message);
                    return result;
                }

                if (metaObjectColumn == null)
                {
                    var message = string.Format("Cannot find MetaObject column by Uid {0}", _kindSettings.RecordsSettings.StorageMetaObjectColumnUid);
                    _logger.LogError(message);
                    result.Error(-1, message);
                    return result;
                }

                if (metaObjectKindColumn == null)
                {
                    var message = string.Format("Cannot find MetaObjectKind column by Uid {0}", _kindSettings.RecordsSettings.StorageKindColumnUid);
                    _logger.LogError(message);
                    result.Error(-1, message);
                    return result;
                }

                var primaryKeyValue = _dataObject.GetPrimaryKey();

                var provider = new DataObjectProvider(_connection, destinationKindSettings, destinationSettings, _dataTypesIndex);
                await provider.DeleteObjectRecordsAsync(metaObjectColumn.Name,
                    destinationSettings.Uid,
                    objectColumn.Name,
                    primaryKeyValue,
                    _transaction);
            }

            return result;
        }


    }
}
