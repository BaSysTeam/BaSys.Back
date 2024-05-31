using System.Data;
using BaSys.Common.Infrastructure;
using BaSys.Core.Abstractions;
using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.DataProviders;
using BaSys.Host.DAL.Helpers;
using BaSys.Host.DAL.TableManagers;
using BaSys.Logging.Abstractions.Abstractions;
using BaSys.Logging.EventTypes;
using BaSys.Metadata.DTOs;
using BaSys.Metadata.Models;
using BaSys.Translation;

namespace BaSys.Core.Services
{
    public sealed class MetaObjectsService : IMetaObjectsService, IDisposable
    {
        private readonly IDbConnection _connection;
        private readonly MetaObjectKindsProvider _kindsProvider;
        private readonly ILoggerService _logger;
        private readonly ISystemObjectProviderFactory _providerFactory;
        private readonly ITableManagerFactory _tableManagerFactory;
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

                var headerChangeAnalyser = new MetaObjectTableChangeAnalyser(savedSettings.Header, newSettings.Header);
                headerChangeAnalyser.Analyze();

                savedSettings.CopyFrom(newSettings);

                try
                {
                    var updateResult = await provider.UpdateSettingsAsync(savedSettings, transaction);
                    result.Success(updateResult, DictMain.ItemUpdated);

                    if (headerChangeAnalyser.NeedAlterTable)
                    {
                        var dataTypes = new PrimitiveDataTypes();
                        var alterTableModel = headerChangeAnalyser.ToAlterModel(dataTypes);

                        var dataObjectTableManager = new DataObjectManager(_connection, kindSettings, savedSettings, dataTypes);
                        await dataObjectTableManager.AlterTableAsync(alterTableModel, transaction);
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
