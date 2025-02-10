using BaSys.DAL.Models.Admin;
using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.ModelConfigurations;
using System.Data;

namespace BaSys.Host.DAL.DataProviders;

public class FileStorageConfigProvider : SystemObjectProviderBase<FileStorageConfig>
{
    public FileStorageConfigProvider(IDbConnection dbConnection) : base(dbConnection, new FileStorageConfigConfiguration())
    {
    }

    //public override async Task<Guid> InsertAsync(FileStorageConfig item, IDbTransaction transaction)
    //{
    //    if (item.Uid == Guid.Empty)
    //    {
    //        item.Uid = Guid.NewGuid();
    //    }

    //    _query = InsertBuilder.Make(_config)
    //        .FillValuesByColumnNames(true)
    //        .Query(_sqlDialect);

    //    var createdCount = await _dbConnection.ExecuteAsync(_query.Text, item, transaction);

    //    return InsertedUid(createdCount, item.Uid);
    //}

    //public override async Task<int> UpdateAsync(FileStorageConfig item, IDbTransaction transaction)
    //{
    //    _query = UpdateBuilder.Make(_config)
    //        .WhereAnd("uid = @uid")
    //        .Query(_sqlDialect);

    //    var result = await _dbConnection.ExecuteAsync(_query.Text, item, transaction);
    //    return result;
    //}
}