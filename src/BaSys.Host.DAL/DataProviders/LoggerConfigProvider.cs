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

        public override async Task<Guid> InsertAsync(LoggerConfig item, IDbTransaction transaction)
        {
            if (item.Uid == Guid.Empty)
            {
                item.Uid = Guid.NewGuid();
            }

            _query = InsertBuilder.Make(_config)
                .FillValuesByColumnNames(true)
                .Query(_sqlDialect);

            var createdCount = await _dbConnection.ExecuteAsync(_query.Text, item, transaction);

            return InsertedUid(createdCount, item.Uid);
        }

        public override async Task<int> UpdateAsync(LoggerConfig item, IDbTransaction transaction)
        {

            _query = SelectBuilder.Make()
                .From(_config.TableName)
                .Select("uid")
                .WhereAnd("loggertype = @loggertype")
                .Parameter("loggertype", (int)item.LoggerType, DbType.Int32)
                .Query(_sqlDialect);
            var result = await _dbConnection.QueryFirstOrDefaultAsync<Guid?>(_query.Text, _query.DynamicParameters, transaction);

            if (result == null)
            {
                 await InsertAsync(item, transaction);
                return 1;
            }
            else
            {
                // unselect all
                _query = UpdateBuilder.Make()
                    .Table(_config.TableName)
                    .Set("isselected", "@false_value")
                    .Parameter("false_value", false, DbType.Boolean)
                    .WhereAnd("1 = 1")
                    .Query(_sqlDialect);
                await _dbConnection.ExecuteAsync(_query.Text, _query.DynamicParameters, transaction);

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
