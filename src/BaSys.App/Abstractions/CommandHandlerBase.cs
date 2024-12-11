using BaSys.App.Features.DataObjectRecords.Commands;
using BaSys.Common.Infrastructure;
using BaSys.Core.Abstractions;
using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.DataProviders;
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

        protected virtual async Task<ResultWrapper<TResult>> ExecuteCommandAsync(TCommand command)
        {
            var result = new ResultWrapper<TResult>();

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
