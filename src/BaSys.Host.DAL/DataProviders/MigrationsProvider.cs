using System.Data;
using BaSys.DAL.Models;
using BaSys.FluentQueries.QueryBuilders;
using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.ModelConfigurations;
using Dapper;

namespace BaSys.Host.DAL.DataProviders;

public class MigrationsProvider : SystemObjectProviderBase<Migration>
{
    public MigrationsProvider(IDbConnection dbConnection) : base(dbConnection, new MigrationsConfiguration())
    {
    }

    //public override async Task<Guid> InsertAsync(Migration item, IDbTransaction transaction)
    //{
    //    if(item.Uid == Guid.Empty)
    //    {
    //        item.Uid = Guid.NewGuid();
    //    }

    //    _query = InsertBuilder.Make(_config)
    //        .FillValuesByColumnNames(true)
    //        .Query(_sqlDialect);

    //    var insertedCount = await _dbConnection.ExecuteAsync(_query.Text, item, transaction);

    //    return InsertedUid(insertedCount, item.Uid);
    //}
    
    //public override async Task<int> UpdateAsync(Migration item, IDbTransaction transaction)
    //{
    //    _query = UpdateBuilder.Make(_config)
    //        .WhereAnd("uid = @uid")
    //        .Query(_sqlDialect);

    //    return await _dbConnection.ExecuteAsync(_query.Text, item, transaction);
    //}
    
    public async Task<Migration?> GetMigrationByMigrationUidAsync(Guid migrationUid, IDbTransaction transaction)
    {
        _query = SelectBuilder.Make()
            .From(_config.TableName)
            .Select("*")
            .WhereAnd("migrationuid = @migrationuid")
            .Parameter("migrationuid", migrationUid)
            .Query(_sqlDialect);

        var result = await _dbConnection.QueryFirstOrDefaultAsync<Migration>(_query.Text, _query.DynamicParameters, transaction);

        return result;
    }
    
    public async Task<int> DeleteByMigrationUidAsync(Guid migrationUid, IDbTransaction? transaction = null)
    {
        _query = DeleteBuilder.Make()
            .Table(_config.TableName)
            .WhereAnd("migrationuid = @migrationuid")
            .Parameter("migrationuid", migrationUid)
            .Query(_sqlDialect);

        var result = await _dbConnection.ExecuteAsync(_query.Text, _query.DynamicParameters, transaction);

        return result;
    }
}