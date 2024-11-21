using BaSys.Common.Helpers;
using BaSys.Common.Infrastructure;
using BaSys.Constructor.Abstractions;
using BaSys.Core.Abstractions;
using BaSys.Core.Services;
using BaSys.DTO.Core;
using BaSys.FluentQueries.Abstractions;
using BaSys.FluentQueries.Models;
using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.DataProviders;
using BaSys.Host.DAL.ModelConfigurations;
using BaSys.Logging.Abstractions.Abstractions;
using BaSys.Metadata.Abstractions;
using BaSys.Metadata.Models;
using System.Data;
using System.Text.Json;

namespace BaSys.Constructor.Services
{
    public sealed class QueriesService : IQueriesService, IDisposable
    {
        private readonly IMainConnectionFactory _connectionFactory;
        private readonly ILoggerService _logger;
        private readonly ISystemObjectProviderFactory _providerFactory;
        private readonly IDataTypesService _dataTypesService;
        private readonly QueriesProvider _queriesProvider;

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

           
            _dataTypesService = new DataTypesService(_providerFactory);
            _dataTypesService.SetUp(_connection);

            _queriesProvider = new QueriesProvider(_connection);

            _logger = logger;
        }


        public async Task<ResultWrapper<DataTableDto>> ExecuteAsync(SelectQueryModelDto queryModelDto)
        {
            var result = new ResultWrapper<DataTableDto>();
        

            try
            {
                MetaObjectStorableSettings? objectSettings = await GetObjectSettingsAsync(queryModelDto);
                var dataTypesIndex = await _dataTypesService.GetIndexAsync(null);
                var selectModel = ConvertToQueryModel(queryModelDto, dataTypesIndex, objectSettings);
                var dataTableDto = await ExecuteQueryAsync(_queriesProvider, selectModel);
                result.Success(dataTableDto);
            }
            catch (Exception ex)
            {
                HandleException(result, ex, _queriesProvider.LastQuery);
            }

            return result;
        }

        private async Task<MetaObjectStorableSettings?> GetObjectSettingsAsync(SelectQueryModelDto queryModelDto)
        {
            if (!queryModelDto.FromExpression.Contains('.'))
                return null;

            var (kindName, objectName) = parseTableName(queryModelDto.FromExpression);
            var kindProvider = _providerFactory.Create<MetaObjectKindsProvider>();
            var kindSettings = await kindProvider.GetSettingsByNameAsync(kindName, null);

            if (kindSettings == null)
                throw new Exception($"Cannot find kind by name: {kindName}");

            var metaObjectProvider = new MetaObjectStorableProvider(_connection, kindSettings.Name);
            var metaObject = await metaObjectProvider.GetItemByNameAsync(objectName, null);

            if (metaObject == null)
                throw new Exception($"Cannot find object by name: {objectName}");

            queryModelDto.FromExpression = DataObjectConfiguration.ComposeTableName(kindSettings.Prefix, objectName);
            return metaObject.ToSettings();
        }

        private async Task<DataTableDto> ExecuteQueryAsync(QueriesProvider provider, SelectModel selectModel)
        {
            var dataTable = await provider.ExecuteAsync(selectModel, null);
            return new DataTableDto(dataTable);
        }

        private void HandleException(ResultWrapper<DataTableDto> result, Exception ex, IQuery? lastQuery)
        {
            var techInfo = $"Message: {ex.Message}. Query: {lastQuery}";
            result.Error(-1, "Cannot execute query.", techInfo);
        }

        private SelectModel ConvertToQueryModel(SelectQueryModelDto dto, 
            IDataTypesIndex dataTypesIndex, 
            MetaObjectStorableSettings? objectSettings)
        {
            var queryModel = new SelectModel();
          

            foreach(var expression in dto.SelectExpressions)
            {
                queryModel.AddSelectExpression(expression);
            }

            if (!queryModel.SelectExpressions.Any())
            {
                queryModel.AddSelectExpression(" * ");
            }

            queryModel.Top = dto.Top;
            queryModel.FromExpression = dto.FromExpression;
            queryModel.WhereExpression = dto.WhereExpression;
            queryModel.OrderByExpression = dto.OrderByExpression;

            foreach (var parameterDto in dto.Parameters)
            {

                if (parameterDto.Value == null)
                {
                    queryModel.AddParameter(parameterDto.Name, null);
                }
                else
                {
                    var jsonValue = (JsonElement)parameterDto.Value;

                    if (parameterDto.DbType.HasValue)
                    {
                        var parameterValue = ValueParser.Parse(jsonValue.ToString(), parameterDto.DbType.Value);
                        queryModel.AddParameter(parameterDto.Name, parameterValue, parameterDto.DbType.Value);
                    }
                    else
                    {
                        // Get DbType from table column.
                        if (objectSettings == null)
                        {
                            throw new ArgumentNullException($"Meta object settings is null. Cannot retrieve DbType");
                        }

                        var fld = objectSettings.Header.GetColumn(parameterDto.Name);
                        if (fld == null)
                        {
                            throw new ArgumentNullException($"Cannot retrieve DbType for parameter {parameterDto.Name}");
                        }

                        var dbType = dataTypesIndex.GetDbType(fld.DataTypeUid);
                        var parameterValue = ValueParser.Parse(jsonValue.ToString(), dbType);
                        queryModel.AddParameter(parameterDto.Name, parameterValue, dbType);
                    }
                 
                }
            }

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

        private Tuple<string, string> parseTableName(string tableName)
        {
            var kindName = string.Empty;
            var objectName = string.Empty;

            var ind = tableName.IndexOf(".");
            if (ind > -1)
            {
                kindName = tableName.Substring(0, ind);
                objectName = tableName.Substring(ind + 1);
            }

            return new Tuple<string, string>(kindName, objectName);
        }
    }
}
