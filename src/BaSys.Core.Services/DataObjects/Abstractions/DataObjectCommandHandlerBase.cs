using BaSys.Common.Infrastructure;
using BaSys.Core.Features.Abstractions;
using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.DataProviders;
using BaSys.Logging.Abstractions.Abstractions;
using System.Data;

namespace BaSys.Core.Features.DataObjects.Abstractions
{
    public abstract class DataObjectCommandHandlerBase<TCommand, TResult>: CommandHandlerBase<TCommand, TResult>, IDisposable
    {
      
        protected readonly ISystemObjectProviderFactory _providerFactory;
        protected readonly IMetadataReader _metadataReader;
        protected readonly ILoggerService _logger;

        protected IDbConnection? _connection;
        protected MetaObjectKindsProvider? _kindProvider;

        protected bool _disposed;

        protected DataObjectCommandHandlerBase( ISystemObjectProviderFactory providerFactory,
            IMetadataReader metadataReader,
            ILoggerService logger)
        {

            _providerFactory = providerFactory;
            _metadataReader = metadataReader;

            _logger = logger;

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

        protected void SetUpConnection(IDbConnection connection)
        {
            _connection = connection;

            _providerFactory.SetUp(_connection);
            _metadataReader.SetUp(_providerFactory);

            _kindProvider = _providerFactory.Create<MetaObjectKindsProvider>();

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
