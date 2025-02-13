using BaSys.Common.Infrastructure;
using BaSys.Core.Features.Abstractions;
using BaSys.Core.Features.DataObjects.Abstractions;
using BaSys.Core.Features.DataObjects.Helpers;
using BaSys.Core.Features.RecordsBuilder;
using BaSys.DTO.App;
using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.DataProviders;
using BaSys.Logging.Abstractions.Abstractions;
using BaSys.Metadata.Models;
using BaSys.Translation;
using System.Data;

namespace BaSys.Core.Features.DataObjects.Commands
{
    public sealed class DataObjectCreateCommandHandler : DataObjectCommandHandlerBase<DataObjectSaveDto, string>, IDataObjectCreateCommandHandler
    {
        public DataObjectCreateCommandHandler(ISystemObjectProviderFactory providerFactory,
            IMetadataReader metadataReder,
            ILoggerService logger):base(providerFactory, metadataReder, logger)
        {
            
        }

        protected async override Task<ResultWrapper<string>> ExecuteCommandAsync(DataObjectSaveDto command, IDbTransaction transaction)
        {
            var result = new ResultWrapper<string>();

            var allKinds = await _kindProvider.GetCollectionAsync(transaction);
            var kind = allKinds.FirstOrDefault(x => x.Uid == command.MetaObjectKindUid);

            if (kind == null)
            {
                result.Error(-1, $"{DictMain.CannotFindMetaObjectKind}", $"MetaObjectKindUid: {command.MetaObjectKindUid}");
                transaction.Rollback();
                return result;
            }

            var objectKindSettings = kind.ToSettings();
            var metaObjectProvider = new MetaObjectStorableProvider(_connection, objectKindSettings.Name);
            var metaObject = await metaObjectProvider.GetItemAsync(command.MetaObjectUid, transaction);

            if (metaObject == null)
            {
                result.Error(-1, $"{DictMain.CannotFindMetaObject}", $"MetaObjectUid: {command.MetaObjectUid}");
                transaction.Rollback();
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
            var newObject = parsedDto.ToObject(metaObjectSettings);

            var provider = new DataObjectProvider(_connection, objectKindSettings, metaObjectSettings, dataTypesIndex);

            var insertedUid = await provider.InsertAsync(newObject, transaction);
            newObject.SetPrimaryKey(insertedUid);
            foreach (var table in newObject.DetailTables)
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

                await tableProvider.InsertAsync(insertedUid, table, transaction);
            }

            if (objectKindSettings.CanCreateRecords)
            {
                var recordsBuilder = new DataObjectsRecordsBuilder(_connection,
                    transaction,
                    objectKindSettings,
                    metaObjectSettings,
                    newObject,
                    _kindProvider,
                    dataTypesIndex,
                    command.LogLevel);
                var buildResult = await recordsBuilder.BuildAsync();
            }

            result.Success(insertedUid, DictMain.ItemSaved);

            return result;
        }

        public IDataObjectCreateCommandHandler SetUp(IDbConnection connection)
        {
            base.SetUpConnection(connection);

            return this;
        }
    }
}
