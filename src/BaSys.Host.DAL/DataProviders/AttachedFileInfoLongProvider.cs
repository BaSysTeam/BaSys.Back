using System.Data;
using BaSys.FluentQueries.QueryBuilders;
using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.ModelConfigurations;
using BaSys.Metadata.Models;
using Dapper;

namespace BaSys.Host.DAL.DataProviders;

public class AttachedFileInfoLongProvider: SystemObjectProviderBase<AttachedFileInfo<long>>
{
    public AttachedFileInfoLongProvider(IDbConnection dbConnection, string kindName) : base(dbConnection, new AttachedFileInfoConfiguration<long>(kindName))
    {
    }

    public override async Task<Guid> InsertAsync(AttachedFileInfo<long> item, IDbTransaction transaction)
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

    public async Task<Guid> InsertDataAsync(AttachedFileInfo<long> item, IDbTransaction transaction)
    {
        _query = InsertBuilder.Make(_config)
            .FillValuesByColumnNames(true)
            .Query(_sqlDialect);

        await _dbConnection.ExecuteAsync(_query.Text, item, transaction);

        // get inserted uid
        return Guid.Empty;
    }

    public override Task<int> UpdateAsync(AttachedFileInfo<long> item, IDbTransaction transaction)
    {
        throw new NotImplementedException();
    }
}