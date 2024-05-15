using System.Data;
using BaSys.FluentQueries.QueryBuilders;
using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.ModelConfigurations;
using BaSys.Metadata.Models;
using Dapper;
using FileInfo = BaSys.Metadata.Models.FileInfo;

namespace BaSys.Host.DAL.DataProviders;

public class AttachedFileInfoGuidProvider: SystemObjectProviderBase<AttachedFileInfo<Guid>>
{
    public AttachedFileInfoGuidProvider(IDbConnection dbConnection, string kindName) : base(dbConnection, new AttachedFileInfoConfiguration<Guid>(kindName))
    {
    }

    public override async Task<int> InsertAsync(AttachedFileInfo<Guid> item, IDbTransaction transaction)
    {
        _query = InsertBuilder.Make(_config)
            .FillValuesByColumnNames(true)
            .Query(_sqlDialect);

        return await _dbConnection.ExecuteAsync(_query.Text, item, transaction);
    }

    public async Task<Guid> InsertDataAsync(AttachedFileInfo<Guid> item, IDbTransaction transaction)
    {
        _query = InsertBuilder.Make(_config)
            .FillValuesByColumnNames(true)
            .Query(_sqlDialect);

        await _dbConnection.ExecuteAsync(_query.Text, item, transaction);

        // get inserted uid
        return Guid.Empty;
    }

    public override Task<int> UpdateAsync(AttachedFileInfo<Guid> item, IDbTransaction transaction)
    {
        throw new NotImplementedException();
    }

    public async Task<List<FileInfo>?> GetAttachedFilesListAsync(Guid metaObjectKindUid,
        Guid metaObjectUid,
        string objectUid)
    {
        _query = SelectBuilder.Make()
            .From(_config.TableName)
            .Select("*")
            .WhereAnd("metaobjectkinduid = @metaobjectkinduid")
            .WhereAnd("metaobjectuid = @metaobjectuid")
            .WhereAnd("objectuid = @objectuid")
            .Parameter("metaobjectkinduid", metaObjectKindUid)
            .Parameter("metaobjectuid", metaObjectUid)
            .Parameter("objectuid", objectUid)
            .Query(_sqlDialect);

        var result = await _dbConnection.QueryAsync<AttachedFileInfo<int>>(_query.Text, _query.DynamicParameters);
        var fileList = result.Select(x => new FileInfo
        {
            Uid = x.Uid,
            FileName = x.FileName,
            IsImage = x.IsImage,
            IsMainImage = x.IsMainImage,
            MimeType = x.MimeType,
            MetaObjectUid = x.MetaObjectUid,
            MetaObjectKindUid = x.MetaObjectKindUid
        }).ToList();

        return fileList;
    }
}