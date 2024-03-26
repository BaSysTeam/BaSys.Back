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

        public MetadataGroupManager(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<int> CreateTableAsync(IDbTransaction transaction = null)
        {
            var builder = CreateTableBuilder.Make()
               .Table("MetadataGroup")
               .PrimaryKey("Uid", DbType.Guid)
               .Column("ParentUid", DbType.Guid)
               .StringColumn("Title", 100)
               .StringColumn("IconClass", 20, false)
               .StringColumn("Memo", 300, false)
               .Column("IsStandard", DbType.Boolean, true);

            var dialectKind = SqlDialectKinds.MsSql;
            if (_connection is NpgsqlConnection)
            {
                dialectKind = SqlDialectKinds.PgSql;
            }

            var query = builder.Query(dialectKind);

            var result = await _connection.ExecuteAsync(query.Text, transaction);

            return result;
        }
    }
}
