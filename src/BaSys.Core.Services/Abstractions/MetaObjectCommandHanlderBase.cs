using BaSys.Common.Infrastructure;
using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.DataProviders;
using BaSys.Logging.Abstractions.Abstractions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.Core.Features.Abstractions
{
    public abstract class MetaObjectCommandHanlderBase<TCommand, TResult> : CommandHandlerBase<TCommand, TResult>, IDisposable
    {
        protected readonly IDbConnection _connection;
        protected readonly ISystemObjectProviderFactory _providerFactory;
        protected readonly MetaObjectKindsProvider _kindProvider;
        protected readonly IMetadataReader _metadataReader;
        protected readonly ILoggerService _logger;

        protected bool _disposed;

        protected MetaObjectCommandHanlderBase(IMainConnectionFactory connectionFactory,
            ISystemObjectProviderFactory providerFactory,
            IMetadataReader metadataReader, 
            ILoggerService logger)
        {
            _logger = logger;

            _connection = connectionFactory.CreateConnection();
            _providerFactory = providerFactory;
            _providerFactory.SetUp(_connection);

            _kindProvider = _providerFactory.Create<MetaObjectKindsProvider>();

            _metadataReader = metadataReader;
            _metadataReader.SetUp(_providerFactory);
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
