using BaSys.App.Features.DataObjectRecords.Commands;
using BaSys.Common.Abstractions;
using BaSys.Common.Infrastructure;
using BaSys.Core.Abstractions;
using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.DataProviders;
using BaSys.Metadata.Models;
using BaSys.Translation;
using System.Data;

namespace BaSys.App.Abstractions
{
    public abstract class CommandHandlerBase<TCommand, TResult>
    {
        protected readonly IDbConnection _connection;
        protected readonly ISystemObjectProviderFactory _providerFactory;
        protected readonly MetaObjectKindsProvider _kindProvider;
        protected readonly IMetadataReader _metadataReader;

        protected bool _disposed;

        protected CommandHandlerBase(IMainConnectionFactory connectionFactory,
            ISystemObjectProviderFactory providerFactory,
            IMetadataReader metadataService)
        {
            _connection = connectionFactory.CreateConnection();
            _providerFactory = providerFactory;
            _providerFactory.SetUp(_connection);

            _kindProvider = _providerFactory.Create<MetaObjectKindsProvider>();

            _metadataReader = metadataService;
            _metadataReader.SetUp(_providerFactory);
            _metadataReader = metadataService;
        }

        public async Task<ResultWrapper<TResult>> ExecuteAsync(TCommand command)
        {
            var result = new ResultWrapper<TResult>();

            try
            {
                result = await ExecuteCommandAsync(command);
            }
            catch (Exception ex)
            {
                result.Error(-1, $"Cannot execute command: {nameof(command)}. Message: {ex.Message}.", ex.StackTrace);
            }

            return result;
        }

        protected abstract Task<ResultWrapper<TResult>> ExecuteCommandAsync(TCommand command);

        protected virtual async Task<MetaObjectKindSettings?> GetKindSettingsAsync(string kindName, IResultWrapper result, IDbTransaction? transaction)
        {
            var kindSettings = await _metadataReader.GetKindSettingsByNameAsync(kindName, transaction);
            if (kindSettings == null)
            {
                if (transaction != null)
                {
                    transaction.Rollback();
                }
                result.Error(-1, $"{DictMain.CannotFindMetaObjectKind}: {kindName}");
            }

            return kindSettings;
        }

        protected virtual async Task<MetaObjectStorableSettings?> GetMetaObjectSettingsAsync(string kindName,
                                                                                            string objectName,
                                                                                            IResultWrapper result,
                                                                                            IDbTransaction? transaction)
        {
            var metaObjectSettings = await _metadataReader.GetMetaObjectSettingsByNameAsync(kindName, objectName, transaction);
            if (metaObjectSettings == null)
            {
                if (transaction != null)
                {
                    transaction.Rollback();
                }
                result.Error(-1, $"{DictMain.CannotFindMetaObject}: {kindName}.{objectName}");
            }

            return metaObjectSettings;
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
