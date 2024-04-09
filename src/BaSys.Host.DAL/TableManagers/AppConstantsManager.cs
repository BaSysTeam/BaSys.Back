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
    public sealed class AppConstantsManager : TableManagerBase
    {
        public AppConstantsManager(IDbConnection connection) : base(connection, "sys_app_constants")
        {
        }

        public override async Task<int> CreateTableAsync(IDbTransaction transaction = null)
        {
            await base.CreateTableAsync(transaction);

            var query = CreateTableBuilder.Make()
               .Table(_tableName)
               .PrimaryKey("Uid", DbType.Guid)
               .Column("DataBaseUid", DbType.Guid, true)
               .StringColumn("ApplicationTitle", 100, true)
               .Query(_sqlDialectKind);

            var result = await _connection.ExecuteAsync(query.Text, null, transaction);

            return result;
        }
    }
}
