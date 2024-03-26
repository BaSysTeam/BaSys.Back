using BaSys.FluentQueries.Enums;
using BaSys.FluentQueries.QueryBuilders;
using Dapper;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.Host.DAL.TableManagers
{
    public sealed class MetadataGroupManager
    {
        private readonly IDbConnection _connection;
        private readonly SqlDialectKinds _sqlDialectKind;
        private string _tableName;

        public MetadataGroupManager(IDbConnection connection)
        {
            _connection = connection;
            _sqlDialectKind = GetDialectKind(_connection);
            _tableName = "MetadataGroups";
        }

        public async Task<int> CreateTableAsync(IDbTransaction transaction = null)
        {
            var query = CreateTableBuilder.Make()
               .Table(_tableName)
               .PrimaryKey("Uid", DbType.Guid)
               .Column("ParentUid", DbType.Guid)
               .StringColumn("Title", 100)
               .StringColumn("IconClass", 20, false)
               .StringColumn("Memo", 300, false)
               .Column("IsStandard", DbType.Boolean, true)
               .Query(_sqlDialectKind);

            var result = await _connection.ExecuteAsync(query.Text, transaction);

            return result;
        }

        public async Task<int> DropTableAsync(IDbTransaction transaction = null)
        {
            var query = DropTableBuilder.Make().Table(_tableName).Query(_sqlDialectKind);

            var result = await _connection.ExecuteAsync(query.Text, transaction);

            return result;

        }

        private SqlDialectKinds GetDialectKind(IDbConnection connection)
        {
            var dialectKind = SqlDialectKinds.MsSql;
            if (connection is NpgsqlConnection)
            {
                dialectKind = SqlDialectKinds.PgSql;
            }

            return dialectKind;
        }
    }
}
