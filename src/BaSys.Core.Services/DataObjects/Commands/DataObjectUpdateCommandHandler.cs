using BaSys.Common.Infrastructure;
using BaSys.Core.Features.Abstractions;
using BaSys.Core.Features.DataObjects.Abstractions;
using BaSys.Core.Features.DataObjects.Helpers;
using BaSys.Core.Features.RecordsBuilder;
using BaSys.DTO.App;
using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.DataProviders;
using BaSys.Logging.Abstractions.Abstractions;
using BaSys.Logging.InMemory;
using BaSys.Metadata.Models;
using BaSys.Metadata.Models.WorkflowModel.Abstractions;
using BaSys.Metadata.Models.WorkflowModel.TriggerEvents;
using BaSys.Translation;
using System.Data;

namespace BaSys.Core.Features.DataObjects.Commands
{
    public sealed class DataObjectUpdateCommandHandler : DataObjectCommandHandlerBase<DataObjectSaveDto, List<InMemoryLogMessage>>, IDataObjectUpdateCommanHandler
    {
        protected override IWorkflowTriggerEvent TriggerEvent => WorkflowTriggerEvents.Update;

        public DataObjectUpdateCommandHandler(ISystemObjectProviderFactory providerFactory,
            IMetadataReader metadataReder,
            ILoggerService logger, 
            IServiceProvider serviceProvider):base (providerFactory, metadataReder, logger, serviceProvider)
        {
            
        }

        protected override async Task<ResultWrapper<List<InMemoryLogMessage>>> ExecuteCommandAsync(DataObjectSaveDto command, IDbTransaction transaction)
        {
            var result = new ResultWrapper<List<InMemoryLogMessage>>();

            var allKinds = await _kindProvider.GetCollectionAsync(transaction);
            var kind = allKinds.FirstOrDefault(x => x.Uid == command.MetaObjectKindUid);

            if (kind == null)
            {
                result.Error(-1, $"{DictMain.CannotFindMetaObjectKind}", $"MetaObjectKindUid: {command.MetaObjectKindUid}");
                return result;
            }
            var objectKindSettings = kind.ToSettings();

            var metaObjectProvider = new MetaObjectStorableProvider(_connection, objectKindSettings.Name);
            var metaObject = await metaObjectProvider.GetItemAsync(command.MetaObjectUid, transaction);

            if (metaObject == null)
            {
                result.Error(-1, $"{DictMain.CannotFindMetaObject}", $"MetaObjectUid: {command.MetaObjectUid}");
                return result;
            }

           
            var metaObjectSettings = metaObject.ToSettings();
            var dataTypesIndex = await _metadataReader.GetIndexAsync(transaction);
            var allMetaObjects = new List<MetaObjectStorable>();

            foreach (var item in allKinds)
            {
                metaObjectProvider = new MetaObjectStorableProvider(_connection, item.Name);
                var metaObjects = await metaObjectProvider.GetCollectionAsync(transaction);

                allMetaObjects.AddRange(metaObjects);
            }

            // Parse header.
            var parsedDto = DataObjectParser.Parse(command.Item, metaObjectSettings, dataTypesIndex);
            var newItem = parsedDto.ToObject(metaObjectSettings);

            var provider = new DataObjectProvider(_connection, objectKindSettings, metaObjectSettings, dataTypesIndex);

            var uid = command.Item.Header[metaObjectSettings.Header.PrimaryKey.Name];
            var objectUid = uid?.ToString() ?? string.Empty;
            
            var savedItem = await provider.GetItemAsync(objectUid, transaction);

            _metadataUid = metaObject.Uid;
            _dataUid = objectUid;
            _dataPresentation = newItem.GetDisplay(kind.Title);

            if (savedItem == null)
            {
                result.Error(-1, $"Cannot find item: {uid}");
                return result;
            }

            savedItem.CopyFrom(newItem);

            var updateResult = await provider.UpdateAsync(savedItem, transaction);
            foreach (var table in savedItem.DetailTables)
            {
                var tableSettings = metaObjectSettings.DetailTables.FirstOrDefault(x => x.Uid == table.Uid);
                if (tableSettings == null)
                {
                    continue;
                }
                var tableProvider = new DataObjectDetailsTableProvider(_connection,
                    objectKindSettings,
                    metaObjectSettings,
                    tableSettings,
                    allKinds,
                    allMetaObjects,
                    dataTypesIndex);

                await tableProvider.UpdateAsync(objectUid, table, transaction);
            }

            var logMessages = new List<InMemoryLogMessage>();
            if (objectKindSettings.CanCreateRecords)
            {
                var recordsBuilder = new DataObjectsRecordsBuilder(_connection,
                    transaction,
                    objectKindSettings,
                    metaObjectSettings,
                    savedItem,
                    _kindProvider,
                    dataTypesIndex,
                    command.LogLevel);
                var buildResult = await recordsBuilder.BuildAsync();

                logMessages.AddRange(recordsBuilder.Messages);
            }

            FillHeaderData(savedItem);
            await GetActiveTriggersAsync(metaObject.Uid, transaction);
           
            result.Success(logMessages, DictMain.ItemSaved);

            return result;
        }

        public IDataObjectUpdateCommanHandler SetUp(IDbConnection connection)
        {
            base.SetUpConnection(connection);

            return this;
        }
    }
}
