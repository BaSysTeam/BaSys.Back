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
            _query = SelectBuilder.Make()
                .From(_config.TableName)
                .Select("uid")
                .WhereAnd("loggertype = @loggertype")
                .Parameter("loggertype", item.LoggerType)
                .Query(_sqlDialect);
            var result = await _dbConnection.QueryFirstOrDefaultAsync<Guid?>(_query.Text, _query.DynamicParameters, transaction);

            if (result == null)
            {
                return await InsertAsync(item, transaction);
            }
            else
            {
                // unselect all
                _query = UpdateBuilder.Make()
                    .Table(_config.TableName)
                    .Set("isselected", "0")
                    .WhereAnd("isselected = 1")
                    .Query(_sqlDialect);
                await _dbConnection.ExecuteAsync(_query.Text, null, transaction);

                // select current & save
                item.IsSelected = true;
                _query = UpdateBuilder.Make(_config)
                    .WhereAnd("loggertype = @loggertype")
                    .Query(_sqlDialect);

                return await _dbConnection.ExecuteAsync(_query.Text, item, transaction);
            }
        }
    }
}
