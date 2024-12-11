using BaSys.App.Abstractions;
using BaSys.Common.Infrastructure;
using BaSys.Core.Abstractions;
using BaSys.Core.Services.RecordsBuilder;
using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.DataProviders;
using BaSys.Translation;
using System.Data;

namespace BaSys.App.Features.DataObjectRecords.Commands
{
    public class CreateRecordsCommandHandler : CommandHandlerBase<CreateRecordsCommand, bool>, ICreateRecordsCommandHandler
    {

        public CreateRecordsCommandHandler(IMainConnectionFactory connectionFactory,
                                           ISystemObjectProviderFactory providerFactory,
                                           IMetadataReader metadataReader) :
            base(connectionFactory,
                 providerFactory,
                 metadataReader)
        {

        }

        protected override async Task<ResultWrapper<bool>> ExecuteCommandAsync(CreateRecordsCommand command)
        {
            var result = new ResultWrapper<bool>();

            _connection.Open();
            using (IDbTransaction transaction = _connection.BeginTransaction())
            {
                var kindSettings = await _metadataReader.GetKindSettingsByNameAsync(command.KindName, transaction);
                if (kindSettings == null)
                {
                    transaction.Rollback();
                    result.Error(-1, $"{DictMain.CannotFindMetaObjectKind}: {command.KindName}");
                    return result;
                }

                var allMetaObjects = await _metadataReader.GetAllMetaObjectsAsync(transaction);

                var metaObjectSettings = await _metadataReader.GetMetaObjectSettingsByNameAsync(command.KindName, command.ObjectName, transaction);
                if (metaObjectSettings == null)
                {
                    transaction.Rollback();
                    result.Error(-1, $"{DictMain.CannotFindMetaObject}: {command.KindName}.{command.ObjectName}");
                    return result;
                }

                var createRecordsColumn = metaObjectSettings.Header.GetColumn(kindSettings.RecordsSettings.SourceCreateRecordsColumnUid);
                if (createRecordsColumn == null)
                {
                    transaction.Rollback();
                    result.Error(-1, $"Cannot find column: {command.KindName}.{command.ObjectName}.{kindSettings.RecordsSettings.SourceCreateRecordsColumnUid}");
                    return result;
                }

                var allKinds = await _metadataReader.GetAllKindsAsync(transaction);
                var dataTypesIndex = await _metadataReader.GetIndexAsync(transaction);
                var provider = new DataObjectProvider(_connection, kindSettings, metaObjectSettings, dataTypesIndex);

                var dataObject = await provider.GetItemAsync(command.ObjectUid, transaction);
                if (dataObject == null)
                {
                    transaction.Rollback();
                    result.Error(-1, $"Cannot find DataObject: {command.KindName}.{command.ObjectName}.{command.ObjectUid}");
                    return result;
                }

                foreach(var tableSettings in metaObjectSettings.DetailTables)
                {
                    var tableProvider = new DataObjectDetailsTableProvider(_connection,
                            kindSettings,
                            metaObjectSettings,
                            tableSettings,
                            allKinds,
                            allMetaObjects,
                            dataTypesIndex);

                    var detailsTable = await tableProvider.GetTableAsync(dataObject.GetPrimaryKey()?.ToString(), transaction);
                    dataObject.DetailTables.Add(detailsTable);
                }

                dataObject.SetValue(createRecordsColumn.Name, true);

                // Create records.
                var recordsBuilder = new DataObjectsRecordsBuilder(_connection,
                    transaction,
                    kindSettings,
                    metaObjectSettings,
                    dataObject,
                    _kindProvider,
                    dataTypesIndex,
                    Common.Enums.EventTypeLevels.Trace);
                var buildResult = await recordsBuilder.BuildAsync();

                // Set flag CreateRecords false.
                await provider.UpdateFieldAsync(dataObject, createRecordsColumn.Name, true, transaction);

                transaction.Commit();
                result.Success(true);

            }

            return result;
        }

    }
}
