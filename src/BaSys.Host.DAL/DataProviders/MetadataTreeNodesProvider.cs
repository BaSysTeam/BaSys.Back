using BaSys.DAL.Models.App;
using BaSys.FluentQueries.Abstractions;
using BaSys.FluentQueries.Enums;
using BaSys.FluentQueries.QueryBuilders;
using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.ModelConfigurations;
using BaSys.Metadata.Models;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.Host.DAL.DataProviders
{
    public sealed class MetadataTreeNodesProvider : SystemObjectProviderBase<MetadataTreeNode>
    {
        public MetadataTreeNodesProvider(IDbConnection dbConnection) : base(dbConnection, new MetadataTreeNodeConfiguration())
        {
        }

        public override async Task<Guid> InsertAsync(MetadataTreeNode item, IDbTransaction transaction)
        {
            if (item.Uid == Guid.Empty)
            {
                item.Uid = Guid.NewGuid();
            }

            _query = InsertBuilder
                .Make(_config)
                .FillValuesByColumnNames(true)
                .Query(_sqlDialect);

            var insertedCount = await _dbConnection.ExecuteAsync(_query.Text, item, transaction);

            return InsertedUid(insertedCount, item.Uid);
        }

        public override async Task<int> UpdateAsync(MetadataTreeNode item, IDbTransaction transaction)
        {
            _query = UpdateBuilder
                .Make(_config)
                .WhereAnd("Uid = @Uid")
                .Query(_sqlDialect);

            return await _dbConnection.ExecuteAsync(_query.Text, item, transaction);
        }

        public async Task<IEnumerable<MetadataTreeNode>> GetChildrenAsync(Guid uid, IDbTransaction transaction)
        {
            _query = SelectBuilder
                .Make()
                .Select("*")
                .From(_config.TableName.ToLower())
                .WhereAnd($"{nameof(MetadataTreeNode.ParentUid)} = @uid")
                .Parameter("uid", uid)
                .Query(_sqlDialect);

            return await _dbConnection.QueryAsync<MetadataTreeNode>(_query.Text, _query.DynamicParameters, transaction);
        }

        public async Task<bool> HasChildrenAsync(Guid uid, IDbTransaction transaction)
        {
            _query = SelectBuilder
                .Make()
                .Select("COUNT(*)")
                .From(_config.TableName.ToLower())
                .WhereAnd($"{nameof(MetadataTreeNode.ParentUid)} = @uid")
                .Parameter("uid", uid)
                .Query(_sqlDialect);

            var result = await _dbConnection.ExecuteScalarAsync<int>(_query.Text, _query.DynamicParameters, transaction);
            return result > 0 ? true : false;
        }
    }
}
