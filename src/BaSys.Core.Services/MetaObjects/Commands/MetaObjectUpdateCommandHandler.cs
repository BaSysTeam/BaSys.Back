using BaSys.Common.Infrastructure;
using BaSys.Core.Features.Abstractions;
using BaSys.DTO.Metadata;
using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.TableChangeAnalyse;
using BaSys.Host.DAL.TableManagers;
using BaSys.Logging.Abstractions.Abstractions;
using BaSys.Logging.EventTypes;
using BaSys.Metadata.Helpers;
using BaSys.Metadata.Validators;
using BaSys.Translation;
using System.Data;

namespace BaSys.Core.Features.MetaObjects.Commands
{
    public sealed class MetaObjectUpdateCommandHandler : MetaObjectCommandHanlderBase<MetaObjectStorableSettingsDto, int>, IMetaObjectUpdateCommandHandler
    {
   

        public MetaObjectUpdateCommandHandler(IMainConnectionFactory connectionFactory,
                                              ISystemObjectProviderFactory providerFactory,
                                              IMetadataReader metadataReader,
                                              ILoggerService logger) : base(connectionFactory, providerFactory, metadataReader, logger)
        {
           
        }

        protected override async Task<ResultWrapper<int>> ExecuteCommandAsync(MetaObjectStorableSettingsDto command, IDbTransaction transaction)
        {
            var result = new ResultWrapper<int>();

            var kindSettings = await _metadataReader.GetKindSettingsAsync(command.MetaObjectKindUid, transaction);

            if (kindSettings == null)
            {
                result.Error(-1, DictMain.CannotFindMetaObjectKind, $"Uid: {command.Uid}");
                transaction.Rollback();
                return result;
            }

            var provider = _providerFactory.CreateMetaObjectStorableProvider(kindSettings.Name);

            var savedSettings = await provider.GetSettingsItemAsync(Guid.Parse(command.Uid), transaction);

            if (savedSettings == null)
            {
                result.Error(-1, DictMain.CannotFindMetaObject, $"Uid: {command.Uid}");
                transaction.Rollback();
                return result;
            }

            var newSettings = command.ToModel();
            newSettings.MetaObjectKindUid = kindSettings.Uid;

            var validator = new MetaObjectStorableSettingsValidator(savedSettings);
            var validationResult = validator.Validate(newSettings);

            if (!validationResult.IsValid)
            {
                result.Error(-1, $"Model is not valid: {validationResult.ToString()}");
                transaction.Rollback();
                return result;
            }

            var allDataTypes = await _metadataReader.GetAllDataTypesAsync(transaction);

            var dataTypeIndex = new DataTypesIndex(allDataTypes);

            var headerChangeAnalyser = new MetaObjectTableChangeAnalyser(savedSettings.Header, newSettings.Header);
            headerChangeAnalyser.Analyze();

            var metaObjectChangeAnalyser = new MetaObjectStorableChangeAnalyser(savedSettings, newSettings, dataTypeIndex);
            metaObjectChangeAnalyser.Analyze();

            var previousSettings = savedSettings.Clone();
            savedSettings.CopyFrom(newSettings);
            var dependencyAnalyser = new DependencyAnalyser();
            dependencyAnalyser.Analyse(savedSettings);

            var updateResult = await provider.UpdateSettingsAsync(savedSettings, transaction);
            result.Success(updateResult, DictMain.ItemUpdated);

            if (headerChangeAnalyser.NeedAlterTable)
            {

                var alterTableModel = headerChangeAnalyser.ToAlterModel(dataTypeIndex);

                var dataObjectTableManager = new DataObjectManager(_connection, kindSettings, savedSettings, dataTypeIndex);
                await dataObjectTableManager.AlterTableAsync(alterTableModel, transaction);
            }

            if (metaObjectChangeAnalyser.Commands.Any())
            {

                foreach (var changeCommand in metaObjectChangeAnalyser.Commands)
                {

                    if (changeCommand is MetaObjectDropTableCommand)
                    {
                        var tableSettings = previousSettings.DetailTables.FirstOrDefault(x => x.Uid == changeCommand.TableUid);
                        if (tableSettings != null)
                        {
                            var detailTableManager = new DataObjectDetailTableManager(_connection, kindSettings, savedSettings, tableSettings, dataTypeIndex);
                            await detailTableManager.DropTableAsync(transaction);
                        }

                    }
                    else if (changeCommand is MetaObjectCreateTableCommand)
                    {
                        var tableSettings = savedSettings.DetailTables.FirstOrDefault(x => x.Uid == changeCommand.TableUid);
                        if (tableSettings != null)
                        {
                            var detailTableManager = new DataObjectDetailTableManager(_connection, kindSettings, savedSettings, tableSettings, dataTypeIndex);

                            await detailTableManager.CreateTableAsync(transaction);
                        }
                    }
                    else if (changeCommand is MetaObjectAlterTableCommand)
                    {
                        var alterCommand = (MetaObjectAlterTableCommand)changeCommand;
                        var tableSettings = savedSettings.DetailTables.FirstOrDefault(x => x.Uid == changeCommand.TableUid);
                        if (tableSettings != null)
                        {
                            var detailTableManager = new DataObjectDetailTableManager(_connection, kindSettings, savedSettings, tableSettings, dataTypeIndex);

                            await detailTableManager.AlterTableAsync(alterCommand.AlterTableModel, transaction);
                        }
                    }
                }
            }

            _logger.Write($"Meta object update {savedSettings}", Common.Enums.EventTypeLevels.Info, EventTypeFactory.MetadataUpdate);

            return result;

        }
    }
}
