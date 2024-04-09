using BaSys.DAL.Models.Admin;
using BaSys.FluentQueries.QueryBuilders;
using BaSys.Host.DAL.Abstractions;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.Host.DAL.DataProviders
{
    public sealed class AppConstantsProvider : SystemObjectProviderBase<AppConstants>
    {
        public AppConstantsProvider(IDbConnection dbConnection) : base(dbConnection, "sys_app_constants")
        {
        }

        public override async Task<int> InsertAsync(AppConstants item, IDbTransaction transaction)
        {
            var query = InsertBuilder.Make()
                .Table(_tableName)
                .Column("databaseuid")
                .Column("applicationtitle")
                .FillValuesByColumnNames(true)
                .Query(_sqlDialect);

            _lastQuery = query;

            var result = await _dbConnection.ExecuteAsync(query.Text, item, transaction);

            return result;
        }

        public override async Task<int> UpdateAsync(AppConstants item, IDbTransaction transaction)
        {
            var query = UpdateBuilder.Make()
                .Table(_tableName)
                .Set("databaseuid")
                .Set("applicationtitle")
                .WhereAnd("uid = @uid")
                .Query(_sqlDialect);

            _lastQuery = query;

            var result = await _dbConnection.ExecuteAsync(query.Text, item, transaction);

            return result;
        }
    }
}
