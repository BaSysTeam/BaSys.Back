using BaSys.FluentQueries.Abstractions;
using BaSys.FluentQueries.Enums;
using BaSys.FluentQueries.QueryBuilders;
using BaSys.Metadata.Models;
using Dapper;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.Host.DAL.DataProviders
{
    public sealed class MetadataGroupProvider
    {
       

        protected readonly IDbConnection _dbConnection;
        protected SqlDialectKinds _sqlDialect;
        protected string _tableName;
        protected IQuery _lastQuery;

        public IQuery LastQuery => _lastQuery;

        public MetadataGroupProvider(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;  
            _sqlDialect = GetDialectKind(dbConnection);
            _tableName = "sys_metadata_groups";
        }

        public async Task<IEnumerable<MetadataGroup>> GetCollectionAsync(IDbTransaction transaction)
        {
            var query = SelectBuilder.Make().From(_tableName).Select("*").Query(_sqlDialect);

            _lastQuery = query;

            var result = await _dbConnection.QueryAsync<MetadataGroup>(query.Text, null, transaction);

            return result;
        }

        public async Task<int> InsertAsync(MetadataGroup item, IDbTransaction transaction)
        {
            var result = 0;

            var query = InsertBuilder.Make()
                .Table(_tableName)
                .Column("parentuid")
                .Column("title")
                .Column("iconclass")
                .Column("memo")
                .Column("isstandard")
                .FillValuesByColumnNames(true).Query(_sqlDialect);

            _lastQuery = query;

            result = await _dbConnection.ExecuteAsync(query.Text, item, transaction);

            return result;
        }

        public async Task<int> UpdateAsync(MetadataGroup item, IDbTransaction transaction)
        {
            var result = 0;

            var query = UpdateBuilder.Make()
                .Table(_tableName)
                .Set("parentuid")
                .Set("title")
                .Set("iconclass")
                .Set("memo")
                .Set("isstandard")
                .WhereAnd("uid = @uid")
                .Query(_sqlDialect);

            _lastQuery = query; 

            result = await _dbConnection.ExecuteAsync(query.Text, item, transaction);

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
