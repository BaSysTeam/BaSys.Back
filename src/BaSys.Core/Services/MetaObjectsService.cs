using System.Data;
using BaSys.Common.Infrastructure;
using BaSys.Core.Abstractions;
using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.DataProviders;
using BaSys.Host.DAL.Helpers;
using BaSys.Host.DAL.TableChangeAnalyse;
using BaSys.Host.DAL.TableManagers;
using BaSys.Logging.Abstractions.Abstractions;
using BaSys.Logging.EventTypes;
using BaSys.Metadata.Abstractions;
using BaSys.DTO.Metadata;
using BaSys.Metadata.Helpers;
using BaSys.Metadata.Models;
using BaSys.Metadata.Validators;
using BaSys.Translation;
using BaSys.DTO.Constructor;
using BaSys.Common.Enums;

namespace BaSys.Core.Services
{
    public sealed class MetaObjectsService : IMetaObjectsService, IDisposable
    {
        private readonly IDbConnection _connection;
        private readonly MetaObjectKindsProvider _kindsProvider;
        private readonly ILoggerService _logger;
        private readonly ISystemObjectProviderFactory _providerFactory;
        private readonly ITableManagerFactory _tableManagerFactory;
        private readonly IDataTypesService _dataTypesService;
        private bool _disposed;

        public MetaObjectsService(IMainConnectionFactory connectionFactory,
            ISystemObjectProviderFactory providerFactory,
            ITableManagerFactory tableManagerFactory,
            ILoggerService logger)
        {
            _connection = connectionFactory.CreateConnection();

            _providerFactory = providerFactory;
            _providerFactory.SetUp(_connection);

            _tableManagerFactory = tableManagerFactory;
            _tableManagerFactory.SetUp(_connection);

            _kindsProvider = _providerFactory.Create<MetaObjectKindsProvider>();

            _dataTypesService = new DataTypesService(providerFactory);
            _dataTypesService.SetUp(_connection);

            _logger = logger;

        }

        public async Task<ResultWrapper<List<MetaObjectStorableSettingsDto>>> GetMetaObjectsAsync(string kindName)
        {
            var result = new ResultWrapper<List<MetaObjectStorableSettingsDto>>();

            var kindSettings = await _kindsProvider.GetSettingsByNameAsync(kindName);
            if (kindSettings == null)
            {
                result.Error(-1, $"{DictMain.CannotFindMetaObjectKind}: {kindName}");
                return result;
            }

            var provider = _providerFactory.CreateMetaObjectStorableProvider(kindSettings.Name);
            var metaObjects = await provider.GetCollectionAsync(null);

            var list = new List<MetaObjectStorableSettingsDto>();
            foreach (var metaObject in metaObjects)
            {
                list.Add(new MetaObjectStorableSettingsDto
                {
                    Uid = metaObject.Uid.ToString(),
                    Title = metaObject.Title,
                    Name = metaObject.Name
                });
            }

            result.Success(list);
            return result;
        }

        public async Task<ResultWrapper<MetaObjectListDto>> GetKindListAsync(string kindName)
        {
            var result = new ResultWrapper<MetaObjectListDto>();

            var kindSettings = await _kindsProvider.GetSettingsByNameAsync(kindName);

            if (kindSettings == null)
            {
                result.Error(-1, $"{DictMain.CannotFindMetaObjectKind}: {kindName}");
                return result;
            }

            var provider = _providerFactory.CreateMetaObjectStorableProvider(kindSettings.Name);

            var items = await provider.GetCollectionAsync(null);

            var listDto = new MetaObjectListDto();
            listDto.Title = kindSettings.Title;
            listDto.MetaObjectKindUid = kindSettings.Uid.ToString();
            listDto.Items = items.Select(x => new MetaObjectDto(x)).ToList();

            result.Success(listDto);

            return result;
        }

        public async Task<ResultWrapper<int>> CreateAsync(MetaObjectStorableSettingsDto settingsDto)
        {
            var result = new ResultWrapper<int>();

            _connection.Open();
            using (IDbTransaction transaction = _connection.BeginTransaction())
            {

                try
                {

                    result = await ExecuteCreateAsync(settingsDto, transaction);
                    _logger.Write($"MetaObject created.", Common.Enums.EventTypeLevels.Info, EventTypeFactory.MetadataCreate);

                    transaction.Commit();

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    result.Error(-1, $"{DictMain.CannotCreateItem}: {ex.Message}", ex.StackTrace);
                    _logger.Write($"Cannot create MetaObject.", Common.Enums.EventTypeLevels.Error, EventTypeFactory.MetadataCreate);
                }
            }

            return result;
        }

        private async Task<ResultWrapper<int>> ExecuteCreateAsync(MetaObjectStorableSettingsDto settingsDto, IDbTransaction transaction)
        {
            var result = new ResultWrapper<int>();

            var metadataKindProvider = _providerFactory.Create<MetaObjectKindsProvider>();
            var kindSettings = await metadataKindProvider.GetSettingsAsync(settingsDto.MetaObjectKindUid, transaction);

            if (kindSettings == null)
            {
                result.Error(-1, DictMain.CannotFindItem, $"Uid: {settingsDto.Uid}");
                transaction.Rollback();
                return result;
            }

            var metaObjectStorableProvider = _providerFactory.CreateMetaObjectStorableProvider(kindSettings.Name);
            var newSettings = settingsDto.ToModel();


            var dataTypesIndex = await _dataTypesService.GetIndexAsync(transaction);
            var dataObjectManager = new DataObjectManager(_connection, kindSettings, newSettings, dataTypesIndex);

            var insertedCount = await metaObjectStorableProvider.InsertSettingsAsync(newSettings, transaction);

            await dataObjectManager.CreateTableAsync(transaction);

            result.Success(insertedCount);

            return result;
        }

        public async Task<ResultWrapper<MetaObjectStorableSettingsDto>> GetSettingsItemAsync(string kindName, string objectName)
        {
            var result = new ResultWrapper<MetaObjectStorableSettingsDto>();

            var kindSettings = await _kindsProvider.GetSettingsByNameAsync(kindName);

            if (kindSettings == null)
            {
                result.Error(-1, $"{DictMain.CannotFindMetaObjectKind}: {kindName}");
                return result;
            }

            var provider = _providerFactory.CreateMetaObjectStorableProvider(kindSettings.Name);
            var metaObject = await provider.GetItemByNameAsync(objectName, null);

            if (metaObject == null)
            {
                result.Error(-1, $"{DictMain.CannotFindMetaObject}: {kindName}.{objectName}");
                return result;
            }

            var settings = metaObject.ToSettings();

            if (kindSettings.IsStandard)
            {
                throw new NotImplementedException($"Not implemented for metaobject kind {kindName}");
            }

            var settingsDto = new MetaObjectStorableSettingsDto(settings, kindSettings);

            result.Success(settingsDto);

            return result;
        }

        public async Task<ResultWrapper<int>> UpdateSettingsItemAsync(MetaObjectStorableSettingsDto settingsDto)
        {
            var result = new ResultWrapper<int>();

            var kindSettings = await _kindsProvider.GetSettingsAsync(settingsDto.MetaObjectKindUid, null);

            if (kindSettings == null)
            {
                result.Error(-1, DictMain.CannotFindMetaObjectKind, $"Uid: {settingsDto.Uid}");
                return result;
            }

            _connection.Open();
            using (IDbTransaction transaction = _connection.BeginTransaction())
            {

                var provider = _providerFactory.CreateMetaObjectStorableProvider(kindSettings.Name);

                var savedSettings = await provider.GetSettingsItemAsync(Guid.Parse(settingsDto.Uid), transaction);

                if (savedSettings == null)
                {
                    result.Error(-1, DictMain.CannotFindMetaObject, $"Uid: {settingsDto.Uid}");
                    return result;
                }

                var newSettings = settingsDto.ToModel();

                var validator = new MetaObjectStorableSettingsValidator(savedSettings);
                var validationResult = validator.Validate(newSettings);

                if (!validationResult.IsValid)
                {
                    result.Error(-1, $"Model is not valid: {validationResult.ToString()}");
                    return result;
                }

                var dataTypeService = new DataTypesService(_providerFactory);
                dataTypeService.SetUp(_connection);
                var allDataTypes = await dataTypeService.GetAllDataTypesAsync(transaction);

                var dataTypeIndex = new DataTypesIndex(allDataTypes);

                var headerChangeAnalyser = new MetaObjectTableChangeAnalyser(savedSettings.Header, newSettings.Header);
                headerChangeAnalyser.Analyze();

                var metaObjectChangeAnalyser = new MetaObjectStorableChangeAnalyser(savedSettings, newSettings, dataTypeIndex);
                metaObjectChangeAnalyser.Analyze();

                var previousSettings = savedSettings.Clone();
                savedSettings.CopyFrom(newSettings);
                var dependencyAnalyser = new DependencyAnalyser();
                dependencyAnalyser.Analyse(savedSettings);

                try
                {
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

                        foreach (var command in metaObjectChangeAnalyser.Commands)
                        {

                            if (command is MetaObjectDropTableCommand)
                            {
                                var tableSettings = previousSettings.DetailTables.FirstOrDefault(x => x.Uid == command.TableUid);
                                if (tableSettings != null)
                                {
                                    var detailTableManager = new DataObjectDetailTableManager(_connection, kindSettings, savedSettings, tableSettings, dataTypeIndex);
                                    await detailTableManager.DropTableAsync(transaction);
                                }

                            }
                            else if (command is MetaObjectCreateTableCommand)
                            {
                                var tableSettings = savedSettings.DetailTables.FirstOrDefault(x => x.Uid == command.TableUid);
                                if (tableSettings != null)
                                {
                                    var detailTableManager = new DataObjectDetailTableManager(_connection, kindSettings, savedSettings, tableSettings, dataTypeIndex);

                                    await detailTableManager.CreateTableAsync(transaction);
                                }
                            }
                            else if (command is MetaObjectAlterTableCommand)
                            {
                                var alterCommand = (MetaObjectAlterTableCommand)command;
                                var tableSettings = savedSettings.DetailTables.FirstOrDefault(x => x.Uid == command.TableUid);
                                if (tableSettings != null)
                                {
                                    var detailTableManager = new DataObjectDetailTableManager(_connection, kindSettings, savedSettings, tableSettings, dataTypeIndex);

                                    await detailTableManager.AlterTableAsync(alterCommand.AlterTableModel, transaction);
                                }
                            }
                        }
                    }

                    _logger.Write($"Meta object update {savedSettings}", Common.Enums.EventTypeLevels.Info, EventTypeFactory.MetadataUpdate);

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    result.Error(-1, DictMain.CannotUpdateItem, ex.Message);
                    _logger.Write($"Meta object update {savedSettings}", Common.Enums.EventTypeLevels.Error, EventTypeFactory.MetadataUpdate);

                    transaction.Rollback();

                }
            }

            return result;
        }

        public async Task<ResultWrapper<int>> DeleteAsync(string kindName, string objectName)
        {
            var result = new ResultWrapper<int>();
            _connection.Open();
            using (IDbTransaction transaction = _connection.BeginTransaction())
            {
                try
                {
                    result = await ExecuteDeleteAsync(kindName, objectName, transaction);
                    if (result.IsOK)
                    {
                        transaction.Commit();
                        _logger.Write($"Meta object delete {kindName}.{objectName}", EventTypeLevels.Info, EventTypeFactory.MetadataDelete);
                    }
                    else
                    {
                        transaction.Rollback();
                        _logger.Write($"Meta object delete {kindName}.{objectName}", EventTypeLevels.Error, EventTypeFactory.MetadataDelete);
                    }
                   
                }
                catch (Exception ex)
                {

                    result.Error(-1, $"{DictMain.CannotDeleteItem}: {ex.Message}", ex.StackTrace);
                    _logger.Write($"Meta object delete {kindName}.{objectName}", EventTypeLevels.Error, EventTypeFactory.MetadataDelete);
                    transaction.Rollback();
                }
            }

            return result;
        }

        private async Task<ResultWrapper<int>> ExecuteDeleteAsync(string kindName, string objectName, IDbTransaction transaction)
        {
            var result = new ResultWrapper<int>();

            var kindSettings = await _kindsProvider.GetSettingsByNameAsync(kindName, transaction);

            if (kindSettings == null)
            {
                result.Error(-1, $"{DictMain.CannotFindMetaObjectKind}: {kindName}");
                return result;
            }

            var metaObjectProvider = _providerFactory.CreateMetaObjectStorableProvider(kindSettings.Name);
            var metaObject = await metaObjectProvider.GetItemByNameAsync(objectName, transaction);

            if (metaObject == null)
            {
                result.Error(-1, $"{DictMain.CannotFindMetaObject}: {kindName}.{objectName}");
                return result;
            }
            var metaObjectSettings = metaObject.ToSettings();

            var dataTypesIndex = await _dataTypesService.GetIndexAsync(transaction);
            var dataObjectProvider = new DataObjectProvider(_connection, kindSettings, metaObjectSettings, dataTypesIndex);
            var dataObjectManager = new DataObjectManager(_connection, kindSettings, metaObjectSettings, dataTypesIndex);

            if (await dataObjectManager.TableExistsAsync(transaction))
            {
                var count = await dataObjectProvider.CountAsync(transaction);

                if (count > 0)
                {
                    result.Error(-1, $"{DictMain.CannotDeleteItem}. {DictMain.ThereAreSomeDataItems}: {count}.");
                    return result;
                }

                await dataObjectManager.DropTableAsync(transaction);
            }

            var deletedCount = await metaObjectProvider.DeleteAsync(metaObject.Uid, transaction);

            result.Success(deletedCount, DictMain.ItemDeleted);

            return result;
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_connection != null)
                        _connection.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                _disposed = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }


    }
}
