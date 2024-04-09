using BaSys.DAL.Models.Logging;
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
    public sealed class LoggerConfigProvider : SystemObjectProviderBase<LoggerConfig>
    {
        public LoggerConfigProvider(IDbConnection dbConnection) : base(dbConnection, "sys_logger_config")
        {
        }

        public override async Task<int> InsertAsync(LoggerConfig item, IDbTransaction transaction)
        {
            var query = InsertBuilder.Make()
                .Table(_tableName)
                .Column("isenabled")
                .Column("loggertype")
                .Column("minimumloglevel")
                .Column("connectionstring")
                .Column("autoclearinterval")
                .FillValuesByColumnNames(true)
                .Query(_sqlDialect);

            _lastQuery = query;

            var result = await _dbConnection.ExecuteAsync(query.Text, item, transaction);

            return result;
        }

        public override async Task<int> UpdateAsync(LoggerConfig item, IDbTransaction transaction)
        {
            var query = UpdateBuilder.Make()
                .Table(_tableName)
                .Set("isenabled")
                .Set("loggertype")
                .Set("minimumloglevel")
                .Set("connectionstring")
                .Set("autoclearinterval")
                .WhereAnd("uid = @uid")
                .Query(_sqlDialect);

            _lastQuery = query;

            var result = await _dbConnection.ExecuteAsync(query.Text, item, transaction);

            return result;
        }
    }
}
