using System.Data;
using BaSys.DAL.Models.App;
using BaSys.FluentQueries.QueryBuilders;
using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.ModelConfigurations;
using Dapper;

namespace BaSys.Host.DAL.DataProviders;

public class UserGroupRoleProvider : SystemObjectProviderBase<UserGroupRole>
{
    public UserGroupRoleProvider(IDbConnection dbConnection) : base(dbConnection, new UserGroupRoleConfiguration())
    {
    }

    public override async Task<int> InsertAsync(UserGroupRole item, IDbTransaction transaction)
    {
        _query = InsertBuilder.Make(_config)
            .FillValuesByColumnNames(true)
            .Query(_sqlDialect);

        var result = await _dbConnection.ExecuteAsync(_query.Text, item, transaction);

        return result;
    }

    public override async Task<int> UpdateAsync(UserGroupRole item, IDbTransaction transaction)
    {
        _query = UpdateBuilder.Make(_config)
            .WhereAnd("uid = @uid")
            .Query(_sqlDialect);

        var result = await _dbConnection.ExecuteAsync(_query.Text, item, transaction);

        return result;
    }
    
    public async Task<IEnumerable<UserGroupRole>> GetCollectionByUserGroupUidAsync(Guid userGroupUid, IDbTransaction? transaction = null)
    {
        _query = SelectBuilder.Make()
            .From(_config.TableName)
            .Select("*")
            .WhereAnd("usergroupuid = @usergroupuid")
            .Parameter("usergroupuid", userGroupUid)
            .Query(_sqlDialect);

        var result = await _dbConnection.QueryAsync<UserGroupRole>(_query.Text, _query.DynamicParameters, transaction);
        return result;
    }
}