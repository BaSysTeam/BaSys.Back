using BaSys.Common.Models;
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
    public sealed class AppConstantsRecordProvider : SystemObjectProviderBase<AppConstantsRecord>
    {
        public AppConstantsRecordProvider(IDbConnection dbConnection) : base(dbConnection, "sys_app_constants_records")
        {
        }

        public override async Task<int> InsertAsync(AppConstantsRecord item, IDbTransaction transaction)
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

        public override async Task<int> UpdateAsync(AppConstantsRecord item, IDbTransaction transaction)
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
