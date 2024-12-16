using BaSys.App.Abstractions;
using BaSys.Common.Abstractions;
using BaSys.Common.Infrastructure;
using BaSys.Core.Features.Abstractions;
using BaSys.Core.Features.RecordsBuilder;
using BaSys.DAL.Models.App;
using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.DataProviders;
using BaSys.Logging.Abstractions.Abstractions;
using BaSys.Logging.InMemory;
using BaSys.Metadata.Abstractions;
using BaSys.Metadata.Models;
using System.Data;

namespace BaSys.App.Features.DataObjectRecords.Commands
{
    public class CreateRecordsCommandHandler : DataObjectCommandHandlerBase<CreateRecordsCommand, List<InMemoryLogMessage>>, ICreateRecordsCommandHandler
    {

        public CreateRecordsCommandHandler(IMainConnectionFactory connectionFactory,
                                           ISystemObjectProviderFactory providerFactory,
                                           IMetadataReader metadataReader,
                                           ILoggerService logger) :
            base(connectionFactory,
                 providerFactory,
                 metadataReader,
                 logger)
        {

        }

        protected override async Task<ResultWrapper<List<InMemoryLogMessage>>> ExecuteCommandAsync(CreateRecordsCommand command, IDbTransaction transaction)
        {
            var result = new ResultWrapper<List<InMemoryLogMessage>>();

            var kindSettings = await GetKindSettingsAsync(command.KindName, result, transaction);
            if (kindSettings == null) return result;

            var metaObjectSettings = await GetMetaObjectSettingsAsync(command.KindName, command.ObjectName, result, transaction);
            if (metaObjectSettings == null) return result;

            var createRecordsColumn = GetCreateRecordsColumn(kindSettings, metaObjectSettings, command, result, transaction);
            if (createRecordsColumn == null) return result;

            var allMetaObjects = await _metadataReader.GetAllMetaObjectsAsync(transaction);
            var allKinds = await _metadataReader.GetAllKindsAsync(transaction);
            var dataTypesIndex = await _metadataReader.GetIndexAsync(transaction);
            var provider = new DataObjectProvider(_connection, kindSettings, metaObjectSettings, dataTypesIndex);

            var dataObject = await GetDataObjectAsync(kindSettings,
                                                      metaObjectSettings,
                                                      command,
                                                      allKinds,
                                                      allMetaObjects,
                                                      dataTypesIndex,
                                                      provider,
                                                      result,
                                                      transaction);

            if (dataObject == null) return result;

            dataObject.SetValue(createRecordsColumn.Name, true);

            // Create records.
            var recordsBuilder = new DataObjectsRecordsBuilder(_connection,
                transaction,
                kindSettings,
                metaObjectSettings,
                dataObject,
                _kindProvider,
                dataTypesIndex,
                command.LogLevel);
            var buildResult = await recordsBuilder.BuildAsync();

            // Set flag CreateRecords false.
            await provider.UpdateFieldAsync(dataObject, createRecordsColumn.Name, true, transaction);

            result.Success(recordsBuilder.Messages.ToList());

            return result;
        }

        private MetaObjectTableColumn? GetCreateRecordsColumn(MetaObjectKindSettings kindSettings,
                                                              MetaObjectStorableSettings metaObjectSettings,
                                                              CreateRecordsCommand command,
                                                              IResultWrapper result,
                                                              IDbTransaction? transaction)
        {
            var createRecordsColumn = metaObjectSettings.Header.GetColumn(kindSettings.RecordsSettings.SourceCreateRecordsColumnUid);
            if (createRecordsColumn == null)
            {
                if (transaction != null)
                {
                    transaction.Rollback();
                }
                result.Error(-1, $"Cannot find column: {command.KindName}.{command.ObjectName}.{kindSettings.RecordsSettings.SourceCreateRecordsColumnUid}");
            }

            return createRecordsColumn;
        }

        private async Task<DataObject?> GetDataObjectAsync(MetaObjectKindSettings kindSettings,
                                                           MetaObjectStorableSettings metaObjectSettings,
                                                           CreateRecordsCommand command,
                                                           IEnumerable<MetaObjectKind> allKinds,
                                                           IEnumerable<MetaObjectStorable> allMetaObjects,
                                                           IDataTypesIndex dataTypesIndex,
                                                           DataObjectProvider provider,
                                                           IResultWrapper result,
                                                           IDbTransaction? transaction)
        {
            var dataObject = await provider.GetItemAsync(command.ObjectUid, transaction);
            if (dataObject == null)
            {
                if (transaction != null)
                {
                    transaction.Rollback();
                }
                result.Error(-1, $"Cannot find DataObject: {command.KindName}.{command.ObjectName}.{command.ObjectUid}");
            }

            foreach (var tableSettings in metaObjectSettings.DetailTables)
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

            return dataObject;
        }

    }
}
