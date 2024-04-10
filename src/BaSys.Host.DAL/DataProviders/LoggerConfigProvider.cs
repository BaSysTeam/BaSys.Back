using BaSys.DAL.Models.Logging;
using BaSys.FluentQueries.QueryBuilders;
using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.ModelConfigurations;
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
        public LoggerConfigProvider(IDbConnection dbConnection) : base(dbConnection, new LoggerConfigConfiguration())
        {
        }

        public override async Task<int> InsertAsync(LoggerConfig item, IDbTransaction transaction)
        {
            _query = InsertBuilder.Make(_config)
                .FillValuesByColumnNames(true)
                .Query(_sqlDialect);

            return await _dbConnection.ExecuteAsync(_query.Text, item, transaction);
        }

        public override async Task<int> UpdateAsync(LoggerConfig item, IDbTransaction transaction)
        {
            _query = UpdateBuilder.Make(_config)
               .WhereAnd("uid = @uid")
               .Query(_sqlDialect);

            return await _dbConnection.ExecuteAsync(_query.Text, item, transaction);
        }
    }
}
