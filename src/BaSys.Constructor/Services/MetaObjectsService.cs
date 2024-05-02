using BaSys.Common.Infrastructure;
using BaSys.Constructor.Abstractions;
using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.DataProviders;
using BaSys.Logging.Abstractions.Abstractions;
using BaSys.Logging.EventTypes;
using BaSys.Metadata.DTOs;
using BaSys.Translation;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Security.AccessControl;

namespace BaSys.Constructor.Services
{
    public sealed class MetaObjectsService :IMetaObjectsService, IDisposable
    {
        private readonly IDbConnection _connection;
        private readonly MetadataKindsProvider _kindsProvider;
        private readonly LoggerService _logger;
        private readonly ISystemObjectProviderFactory _providerFactory;
        private bool _disposed;

        public MetaObjectsService(IMainConnectionFactory connectionFactory,
            ISystemObjectProviderFactory providerFactory,
            LoggerService logger)
        {
            _connection = connectionFactory.CreateConnection();

            _providerFactory = providerFactory;
            _providerFactory.SetUp(_connection);

            _kindsProvider = _providerFactory.Create<MetadataKindsProvider>();

            _logger = logger;

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

            var provider = _providerFactory.CreateMetaObjectStorableProvider(kindSettings.GetNamePlural());
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

            var kindSettings = await _kindsProvider.GetSettingsAsync( settingsDto.MetaObjectKindUid, null);

            if (kindSettings == null)
            {
                result.Error(-1, DictMain.CannotFindMetaObjectKind, $"Uid: {settingsDto.Uid}");
                return result;
            }

            var provider = _providerFactory.CreateMetaObjectStorableProvider(kindSettings.GetNamePlural());

            var savedSettings = await provider.GetSettingsItemAsync(Guid.Parse(settingsDto.Uid), null);

            if (savedSettings == null)
            {
                result.Error(-1, DictMain.CannotFindMetaObject, $"Uid: {settingsDto.Uid}");
                return result;
            }

            var newSettings = settingsDto.ToModel();
            savedSettings.CopyFrom(newSettings);

            try
            {
                var updateResult = await provider.UpdateSettingsAsync(savedSettings, null);
                result.Success(updateResult, DictMain.ItemUpdated);

                _logger.Write($"Meta object update {savedSettings}", Common.Enums.EventTypeLevels.Info, EventTypeFactory.MetadataUpdate);
            }
            catch(Exception ex)
            {
                result.Error(-1, DictMain.CannotUpdateItem, ex.Message );
                _logger.Write($"Meta object update {savedSettings}", Common.Enums.EventTypeLevels.Error, EventTypeFactory.MetadataUpdate);

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
