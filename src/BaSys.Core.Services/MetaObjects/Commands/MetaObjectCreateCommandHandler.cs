using BaSys.Common.Infrastructure;
using BaSys.Core.Features.Abstractions;
using BaSys.DTO.Metadata;
using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.TableManagers;
using BaSys.Logging.Abstractions.Abstractions;
using BaSys.Translation;
using System.Data;

namespace BaSys.Core.Features.MetaObjects.Commands
{
    public sealed class MetaObjectCreateCommandHandler : MetaObjectCommandHandlerBase<MetaObjectStorableSettingsDto, int>, IMetaObjectCreateCommandHandler
    {
        public MetaObjectCreateCommandHandler(IMainConnectionFactory connectionFactory,
            ISystemObjectProviderFactory providerFactory,
            IMetadataReader metadataReder, 
            ILoggerService logger)
            : base(connectionFactory,
                  providerFactory,
                  metadataReder, 
                  logger)
        {
        }

        protected override async Task<ResultWrapper<int>> ExecuteCommandAsync(MetaObjectStorableSettingsDto command, IDbTransaction transaction)
        {
            var result = new ResultWrapper<int>();

            var kindSettings = await _metadataReader.GetKindSettingsAsync(command.MetaObjectKindUid, transaction);
            if (kindSettings == null)
            {
                result.Error(-1, DictMain.CannotFindItem, $"Uid: {command.Uid}");
                transaction.Rollback();
                return result;
            }

            var newSettings = command.ToModel();
            newSettings.MetaObjectKindUid = kindSettings.Uid;

            var dataTypesIndex = await _metadataReader.GetIndexAsync(transaction);

            var metaObjectStorableProvider = _providerFactory.CreateMetaObjectStorableProvider(kindSettings.Name);
            var dataObjectManager = new DataObjectManager(_connection, kindSettings, newSettings, dataTypesIndex);

            // Create table for header.
            await dataObjectManager.CreateTableAsync(transaction);
            foreach (var tableSettings in newSettings.DetailTables)
            {
                // Create table for details table.
                var detailTableManager = new DataObjectDetailTableManager(_connection,
                    kindSettings,
                    newSettings,
                    tableSettings,
                    dataTypesIndex);

                await detailTableManager.CreateTableAsync(transaction);
            }

            // Save settings of new MetaObject.
            var insertedCount = await metaObjectStorableProvider.InsertSettingsAsync(newSettings, transaction);

            result.Success(insertedCount);

            return result;
        }
    }
}
