using BaSys.Common.Infrastructure;
using BaSys.Constructor.Abstractions;
using BaSys.Core.Abstractions;
using BaSys.FluentQueries.Models;
using BaSys.DTO.Core;
using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.DataProviders;
using BaSys.Logging.Abstractions.Abstractions;
using Microsoft.DotNet.Scaffolding.Shared;
using System.Data;

namespace BaSys.Constructor.Services
{
    public sealed class QueriesService : IQueriesService, IDisposable
    {
        private readonly IMainConnectionFactory _connectionFactory;
        private readonly ILoggerService _logger;
        private readonly ISystemObjectProviderFactory _providerFactory;

        private readonly IDbConnection _connection;
        private bool _disposed;

        public QueriesService(IMainConnectionFactory connectionFactory,
            ISystemObjectProviderFactory providerFactory,
            ILoggerService logger)
        {
            _connectionFactory = connectionFactory;
            _providerFactory = providerFactory;

            _connection = _connectionFactory.CreateConnection();
            _providerFactory.SetUp(_connection);

            _logger = logger;
        }

        public async Task<ResultWrapper<DataTableDto>> ExecuteAsync(SelectQueryModelDto queryModelDto)
        {
            var result = new ResultWrapper<DataTableDto>();

            var selectModel = ConvertToQueryModel(queryModelDto);

            var provider = new QueriesProvider(_connection);

            try
            {
                var dataTable = await provider.ExecuteAsync(selectModel, null);
                var dataTableDto = new DataTableDto(dataTable);

                result.Success(dataTableDto);
               
            }
            catch (Exception ex) {

                var techInfo = $"Message: {ex.Message}. Query: {provider.LastQuery}";
                result.Error(-1, "Cannot execute query.", $"Message: {ex.Message}. Query: {provider.LastQuery}");
            }

            return result;
        }

        private SelectModel ConvertToQueryModel(SelectQueryModelDto dto)
        {
            var queryModel = new SelectModel();
            queryModel.AddSelectExpression(" * ");
            queryModel.FromExpression = dto.FromExpression;
            queryModel.WhereExpression = dto.WhereExpression;
            queryModel.OrderByExpression = dto.OrderByExpression;

            return queryModel;
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
