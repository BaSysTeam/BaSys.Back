using BaSys.DAL.Models.Admin;
using BaSys.FluentQueries.QueryBuilders;
using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.ModelConfigurations;
using Dapper;
using System.Data;

namespace BaSys.Host.DAL.DataProviders
{
    public sealed class AppConstantsProvider : SystemObjectProviderBase<AppConstants>
    {
        public AppConstantsProvider(IDbConnection dbConnection) : base(dbConnection, new AppConstantsConfiguration())
        {
        }

        public  async Task<AppConstants?> GetConstantsAsync(IDbTransaction? transaction)
        {
            _query = SelectBuilder.Make()
              .From(_config.TableName)
              .Select("*")
              .Top(1)
              .Query(_sqlDialect);

            var result = await _dbConnection.QueryFirstOrDefaultAsync<AppConstants>(_query.Text, _query.DynamicParameters, transaction);

            return result;
        }

    }
}
