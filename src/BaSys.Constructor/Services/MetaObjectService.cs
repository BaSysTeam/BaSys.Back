﻿using BaSys.Common.Infrastructure;
using BaSys.Constructor.Abstractions;
using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.DataProviders;
using BaSys.Logging.Abstractions.Abstractions;
using BaSys.Metadata.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace BaSys.Constructor.Services
{
    public sealed class MetaObjectService :IMetaObjectService, IDisposable
    {
        private readonly IDbConnection _connection;
        private readonly MetadataKindsProvider _kindsProvider;
        private readonly LoggerService _logger;
        private readonly ISystemObjectProviderFactory _providerFactory;
        private bool _disposed;

        public MetaObjectService(IMainConnectionFactory connectionFactory,
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
                result.Error(-1, $"Cannot find metaobject kind: {kindName}");
                return result;
            }

            var provider = _providerFactory.CreateMetaObjectStorableProvider(kindSettings.GetNamePlural());
            var metaObject = await provider.GetItemByNameAsync(objectName, null);

            if (metaObject == null)
            {
                result.Error(-1, $"Cannot find  metaobject: {kindName}.{objectName}");
                return result;
            }

            var settings = metaObject.ToSettings();

            if (kindSettings.IsStandard)
            {
                throw new NotImplementedException($"Not implemented for metaobject kind {kindName}");
            }

            var settingsDto = new MetaObjectStorableSettingsDto
            {
                Title = settings.Title,
                Name = settings.Name,
                MetaObjectKindUid = kindSettings.Uid,
                MetaObjectKindTitle = kindSettings.Title,
                Memo = settings.Memo,
                IsActive = settings.IsActive
            };

            result.Success(settingsDto);

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