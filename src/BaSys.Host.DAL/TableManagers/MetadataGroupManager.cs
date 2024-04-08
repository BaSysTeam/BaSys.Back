using BaSys.FluentQueries.Enums;
using BaSys.FluentQueries.QueryBuilders;
using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.ModelConfigurations;
using BaSys.Host.DAL.QueryResults;
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
    public sealed class MetadataGroupManager : TableManagerBase
    {
        private readonly MetadataGroupConfiguration _config;

        public MetadataGroupManager(IDbConnection connection):base(connection, "sys_metadata_groups")
        {
            _config = new MetadataGroupConfiguration();
        }

        public override async Task<int> CreateTableAsync(IDbTransaction transaction = null)
        {
            await base.CreateTableAsync(transaction);

            //var query = CreateTableBuilder.Make()
            //   .Table(_tableName)
            //   .PrimaryKey("Uid", DbType.Guid)
            //   .Column("ParentUid", DbType.Guid, false)
            //   .StringColumn("Title", 100, true)
            //   .StringColumn("IconClass", 20, false)
            //   .StringColumn("Memo", 300, false)
            //   .Column("IsStandard", DbType.Boolean, true)
            //   .Query(_sqlDialectKind);

            var query = CreateTableBuilder.Make(_config).Query(_sqlDialectKind);

            var result = await _connection.ExecuteAsync(query.Text, null, transaction);

            return result;
        }
      
    }
}
