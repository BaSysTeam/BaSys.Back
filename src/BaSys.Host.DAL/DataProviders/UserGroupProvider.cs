using System.Data;
using BaSys.DAL.Models.App;
using BaSys.FluentQueries.Abstractions;
using BaSys.FluentQueries.QueryBuilders;
using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.ModelConfigurations;
using Dapper;

namespace BaSys.Host.DAL.DataProviders;

public class UserGroupProvider : SystemObjectProviderBase<UserGroup>
{
    public UserGroupProvider(IDbConnection dbConnection) : base(dbConnection, new UserGroupConfiguration())
    {
    }

    public override async Task<Guid> InsertAsync(UserGroup item, IDbTransaction? transaction)
    {
        if (item.Uid == Guid.Empty)
        {
            item.Uid = Guid.NewGuid();
        }

        _query = InsertBuilder.Make(_config)
            .FillValuesByColumnNames(true)
            .Query(_sqlDialect);

        var insertedCount = await _dbConnection.ExecuteAsync(_query.Text, item, transaction);

        return InsertedUid(insertedCount, item.Uid);
    }

    public override async Task<int> UpdateAsync(UserGroup item, IDbTransaction? transaction)
    {
        _query = UpdateBuilder.Make(_config)
            .WhereAnd("uid = @uid")
            .Query(_sqlDialect);

        var result = await _dbConnection.ExecuteAsync(_query.Text, item, transaction);

        return result;
    }
}