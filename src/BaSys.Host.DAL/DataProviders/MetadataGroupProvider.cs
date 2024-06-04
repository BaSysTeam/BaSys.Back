using BaSys.FluentQueries.Abstractions;
using BaSys.FluentQueries.Enums;
using BaSys.FluentQueries.QueryBuilders;
using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.ModelConfigurations;
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

        public MetadataGroupProvider(IDbConnection dbConnection) : base(dbConnection, new MetadataGroupConfiguration())
        {
            _config = new MetadataGroupConfiguration();
        }

        public override async Task<Guid> InsertAsync(MetadataGroup item, IDbTransaction transaction)
        {
            if (item.Uid == Guid.Empty)
            {
                item.Uid = Guid.NewGuid();
            }

            _query = InsertBuilder.Make(_config)
              .FillValuesByColumnNames(true).Query(_sqlDialect);

            var insertedCount = await _dbConnection.ExecuteAsync(_query.Text, item, transaction);

            return InsertedUid(insertedCount, item.Uid);
        }

        public override async Task<int> UpdateAsync(MetadataGroup item, IDbTransaction transaction)
        {
            var result = 0;

            _query = UpdateBuilder.Make(_config)
              .WhereAnd("uid = @uid")
              .Query(_sqlDialect);

            result = await _dbConnection.ExecuteAsync(_query.Text, item, transaction);

            return result;
        }

        public async Task<bool> HasChildrenAsync(Guid parentUid, IDbTransaction transaction)
        {
            var query = SelectBuilder
                .Make()
                .Select("COUNT(*)")
                .From(_config.TableName.ToLower())
                .WhereAnd("ParentUid = @parentUid")
                .Query(_sqlDialect);

            var result = await _dbConnection.ExecuteScalarAsync<int>(query.Text, new { parentUid });
            return result > 0 ? true : false;
        }
    }
}
