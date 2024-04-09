using BaSys.FluentQueries.QueryBuilders;
using BaSys.Host.DAL.Abstractions;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.Host.DAL.TableManagers
{
    public sealed class LoggerConfigManager : TableManagerBase
    {
        public LoggerConfigManager(IDbConnection connection) : base(connection, "sys_logger_config")
        {
        }

        public override async Task<int> CreateTableAsync(IDbTransaction transaction = null)
        {
            await base.CreateTableAsync(transaction);

            var query = CreateTableBuilder.Make()
               .Table(_tableName)
               .PrimaryKey("Uid", DbType.Guid)
               .Column("IsEnabled", DbType.Boolean, true)
               .Column("LoggerType", DbType.Byte, false)
               .Column("MinimumLogLevel", DbType.Byte, true)
               .StringColumn("ConnectionString", 300, false)
               .Column("AutoClearInterval", DbType.Byte, true)
               .Query(_sqlDialectKind);

            var result = await _connection.ExecuteAsync(query.Text, null, transaction);

            return result;
        }
    }
}
