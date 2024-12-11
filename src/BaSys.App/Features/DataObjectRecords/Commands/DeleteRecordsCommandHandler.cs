using BaSys.App.Abstractions;
using BaSys.Common.Infrastructure;
using BaSys.Core.Abstractions;
using BaSys.Core.Services;
using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.DataProviders;
using System.Data;

namespace BaSys.App.Features.DataObjectRecords.Commands
{
    public sealed class DeleteRecordsCommandHandler:IDeleteRecordsCommandHandler, IDisposable
    {
        private readonly IDbConnection _connection;
        private readonly ISystemObjectProviderFactory _providerFactory;
        private readonly MetaObjectKindsProvider _kindProvider;
        private readonly IDataTypesService _dataTypesService;
        private bool _disposed;

        public DeleteRecordsCommandHandler(IMainConnectionFactory connectionFactory,
            ISystemObjectProviderFactory providerFactory)
        {
            _connection = connectionFactory.CreateConnection();
            _providerFactory = providerFactory;
            _providerFactory.SetUp(_connection);

            _kindProvider = _providerFactory.Create<MetaObjectKindsProvider>();

            _dataTypesService = new DataTypesService(providerFactory);
            _dataTypesService.SetUp(_connection);
        }

        public async Task<ResultWrapper<bool>> ExecuteAsync(DeleteRecordsCommand command)
        {
            var result = new ResultWrapper<bool>();

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

        private async Task<ResultWrapper<bool>> ExecuteCommandAsync(DeleteRecordsCommand command)
        {
            var result = new ResultWrapper<bool>();

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
