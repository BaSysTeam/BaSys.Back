using BaSys.Common.Abstractions;
using BaSys.Common.Infrastructure;
using BaSys.Core.Features.Abstractions;
using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.DataProviders;
using BaSys.Logging.Abstractions.Abstractions;
using BaSys.Metadata.Models;
using BaSys.Translation;
using System.Data;

namespace BaSys.App.Abstractions
{
    public abstract class DataObjectCommandHandlerBase<TCommand, TResult>: CommandHandlerBase<TCommand, TResult>, IDisposable
    {
        protected readonly IDbConnection _connection;
        protected readonly ISystemObjectProviderFactory _providerFactory;
        protected readonly MetaObjectKindsProvider _kindProvider;
        protected readonly IMetadataReader _metadataReader;
        protected readonly ILoggerService _logger;

        protected bool _disposed;

        protected DataObjectCommandHandlerBase(IMainConnectionFactory connectionFactory,
            ISystemObjectProviderFactory providerFactory,
            IMetadataReader metadataService, 
            ILoggerService logger)
        {
            _logger = logger;

            _connection = connectionFactory.CreateConnection();
            _providerFactory = providerFactory;
            _providerFactory.SetUp(_connection);

            _kindProvider = _providerFactory.Create<MetaObjectKindsProvider>();

            _metadataReader = metadataService;
            _metadataReader.SetUp(_providerFactory);
            _metadataReader = metadataService;
        }

        public override async Task<ResultWrapper<TResult>> ExecuteAsync(TCommand command)
        {
            var result = new ResultWrapper<TResult>();
            _connection.Open();
            using (IDbTransaction transaction = _connection.BeginTransaction())
            {
                try
                {
                    result = await ExecuteCommandAsync(command, transaction);
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    result.Error(-1, $"Cannot execute command: {nameof(command)}. Message: {ex.Message}.", ex.StackTrace);
                }
            }


            return result;
        }

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
