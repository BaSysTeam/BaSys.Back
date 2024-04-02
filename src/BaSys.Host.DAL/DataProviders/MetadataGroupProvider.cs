using BaSys.FluentQueries.Abstractions;
using BaSys.FluentQueries.Enums;
using BaSys.FluentQueries.QueryBuilders;
using BaSys.Host.DAL.Abstractions;
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
    public sealed class MetadataGroupProvider : SystemObjectProviderBase<MetadataGroup>
    {

        public MetadataGroupProvider(IDbConnection dbConnection):base(dbConnection, "sys_metadata_groups")
        {
           
        }

        public override async Task<IEnumerable<MetadataGroup>> GetCollectionAsync(IDbTransaction transaction)
        {
            var query = SelectBuilder.Make().From(_tableName).Select("*").Query(_sqlDialect);

            _lastQuery = query;

            var result = await _dbConnection.QueryAsync<MetadataGroup>(query.Text, null, transaction);

            return result;
        }

        public override async Task<MetadataGroup> GetItemAsync(Guid uid, IDbTransaction transaction)
        {
            var query = SelectBuilder.Make()
                .From(_tableName)
                .Select("*")
                .WhereAnd("uid = @uid")
                .Parameter("uid", uid)
                .Query(_sqlDialect);

            _lastQuery = query;

            var result = await _dbConnection.QueryFirstOrDefaultAsync<MetadataGroup>(query.Text, query.DynamicParameters, transaction);

            return result;
        }

        public override async Task<int> InsertAsync(MetadataGroup item, IDbTransaction transaction)
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

        public override async Task<int> UpdateAsync(MetadataGroup item, IDbTransaction transaction)
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
       
    }
}
