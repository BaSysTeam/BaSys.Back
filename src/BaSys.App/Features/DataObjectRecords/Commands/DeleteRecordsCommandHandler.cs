using BaSys.App.Abstractions;
using BaSys.Common.Abstractions;
using BaSys.Common.Infrastructure;
using BaSys.Core.Features.Abstractions;
using BaSys.Core.Features.RecordsBuilder;
using BaSys.DAL.Models.App;
using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.DataProviders;
using BaSys.Logging.Abstractions.Abstractions;
using BaSys.Metadata.Models;
using System.Data;

namespace BaSys.App.Features.DataObjectRecords.Commands
{
    public sealed class DeleteRecordsCommandHandler : DataObjectCommandHandlerBase<DeleteRecordsCommand, bool>, IDeleteRecordsCommandHandler
    {


        public DeleteRecordsCommandHandler(IMainConnectionFactory connectionFactory,
                                           ISystemObjectProviderFactory providerFactory,
                                           IMetadataReader metadataReader, 
                                           ILoggerService logger) :
            base(connectionFactory,
                providerFactory,
                metadataReader, 
                logger)
        {

        }

        protected override async Task<ResultWrapper<bool>> ExecuteCommandAsync(DeleteRecordsCommand command, IDbTransaction transaction)
        {
            var result = new ResultWrapper<bool>();


            var kindSettings = await GetKindSettingsAsync(command.KindName, result, transaction);
            if (kindSettings == null) return result;

            var metaObjectSettings = await GetMetaObjectSettingsAsync(command.KindName, command.ObjectName, result, transaction);
            if (metaObjectSettings == null) return result;

            var createRecordsColumn = GetCreateRecordsColumn(kindSettings, metaObjectSettings, command, result, transaction);
            if (createRecordsColumn == null) return result;

            var dataTypesIndex = await _metadataReader.GetIndexAsync(transaction);
            var allMetaObjects = await _metadataReader.GetAllMetaObjectsAsync(transaction);
            var provider = new DataObjectProvider(_connection, kindSettings, metaObjectSettings, dataTypesIndex);

            var mockObject = new DataObject(metaObjectSettings, dataTypesIndex);
            mockObject.SetPrimaryKey(command.ObjectUid);

            // Delete records.
            var recordsBuilder = new DataObjectsRecordsBuilder(_connection,
                transaction,
                kindSettings,
                metaObjectSettings,
                mockObject,
                _kindProvider,
                dataTypesIndex,
                Common.Enums.EventTypeLevels.Trace);
            var buildResult = await recordsBuilder.DeleteAsync();

            // Set flag CreateRecords false.
            await provider.UpdateFieldAsync(mockObject, createRecordsColumn.Name, false, transaction);

            result.Success(true);

            return result;
        }

        private MetaObjectTableColumn? GetCreateRecordsColumn(MetaObjectKindSettings kindSettings,
                                                              MetaObjectStorableSettings metaObjectSettings,
                                                              DeleteRecordsCommand command,
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

    }
}
