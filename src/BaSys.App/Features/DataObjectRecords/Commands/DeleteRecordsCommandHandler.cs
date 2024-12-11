using BaSys.App.Abstractions;
using BaSys.Common.Infrastructure;
using BaSys.Core.Abstractions;
using BaSys.Core.Services.RecordsBuilder;
using BaSys.DAL.Models.App;
using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.DataProviders;
using BaSys.Translation;
using System.Data;

namespace BaSys.App.Features.DataObjectRecords.Commands
{
    public sealed class DeleteRecordsCommandHandler : CommandHandlerBase<DeleteRecordsCommand, bool>, IDeleteRecordsCommandHandler
    {
      

        public DeleteRecordsCommandHandler(IMainConnectionFactory connectionFactory,
                                           ISystemObjectProviderFactory providerFactory,
                                           IMetadataReader metadataReader) : 
            base(connectionFactory, 
                providerFactory,
                metadataReader)
        {
           
        }

        protected override async Task<ResultWrapper<bool>> ExecuteCommandAsync(DeleteRecordsCommand command)
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

                var dataTypesIndex = await _metadataReader.GetIndexAsync(transaction);
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

                transaction.Commit();
                result.Success(true);

            }

            return result;
        }

    }
}
