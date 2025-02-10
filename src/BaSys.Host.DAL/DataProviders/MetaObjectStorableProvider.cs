using System.Data;
using BaSys.FluentQueries.Abstractions;
using BaSys.FluentQueries.QueryBuilders;
using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.ModelConfigurations;
using BaSys.Metadata.Models;
using Dapper;
using MessagePack;

namespace BaSys.Host.DAL.DataProviders;

public class MetaObjectStorableProvider : SystemObjectProviderBase<MetaObjectStorable>
{
    public MetaObjectStorableProvider(IDbConnection dbConnection, string kindNamePlural) : base(dbConnection, new MetadataObjectConfiguration(kindNamePlural))
    {
    }

    public override async Task<IEnumerable<MetaObjectStorable>> GetCollectionAsync(IDbTransaction transaction)
    {
        _query = SelectBuilder.Make().From(_config.TableName).Select("*").OrderBy("title").Query(_sqlDialect);

        var result = await _dbConnection.QueryAsync<MetaObjectStorable>(_query.Text, null, transaction);

        return result;
    }

    //public override async Task<Guid> InsertAsync(MetaObjectStorable item, IDbTransaction transaction)
    //{
    //    if (item.Uid == Guid.Empty)
    //    {
    //        item.Uid = Guid.NewGuid();
    //    }

    //    _query = InsertBuilder.Make(_config)
    //        .FillValuesByColumnNames(true)
    //        .Query(_sqlDialect);

    //    var insertedCount = await _dbConnection.ExecuteAsync(_query.Text, item, transaction);

    //    return InsertedUid(insertedCount, item.Uid);
    //}

    //public override async Task<int> UpdateAsync(MetaObjectStorable item, IDbTransaction transaction)
    //{
    //    _query = UpdateBuilder.Make(_config)
    //        .WhereAnd("uid = @uid")
    //        .Query(_sqlDialect);

        

    //    var result = await _dbConnection.ExecuteAsync(_query.Text, item, transaction);

    //    return result;
    //}

    /// <summary>
    /// Insert all colunms
    /// </summary>
    /// <param name="settings"></param>
    /// <param name="transaction"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<int> InsertSettingsAsync(MetaObjectStorableSettings settings, IDbTransaction? transaction)
    {
        var settingsArr = MessagePackSerializer.Serialize(settings);
        var item = new MetaObjectStorable
        {
            Uid = settings.Uid,
            Name = settings.Name,
            Title = settings.Title,
            Memo = settings.Memo,
            MetaObjectKindUid = settings.MetaObjectKindUid,
            IsActive = settings.IsActive,
            Version = settings.Version,
            SettingsStorage = settingsArr
        };
        await InsertAsync(item, transaction);
        var result = 1;

        return result;
    }

    /// <summary>
    /// Update all columns
    /// </summary>
    /// <param name="settings"></param>
    /// <param name="transaction"></param>
    /// <returns></returns>
    public async Task<int> UpdateSettingsAsync(MetaObjectStorableSettings settings, IDbTransaction? transaction)
    {
        var settingsArr = MessagePackSerializer.Serialize(settings);
        var item = new MetaObjectStorable
        {
            Uid = settings.Uid,
            Name = settings.Name,
            Title = settings.Title,
            Memo = settings.Memo,
            MetaObjectKindUid = settings.MetaObjectKindUid,
            IsActive = settings.IsActive,
            Version = settings.Version,
            SettingsStorage = settingsArr
        };
        var result = await UpdateAsync(item, transaction);

        return result;
    }

    public async Task<MetaObjectStorableSettings?> GetSettingsItemAsync(Guid uid, IDbTransaction? transaction)
    {
        _query = SelectBuilder.Make()
            .From(_config.TableName)
            .Select("settingsstorage")
            .WhereAnd("uid = @uid")
            .Parameter("uid", uid)
            .Query(_sqlDialect);

        var item = await GetItemAsync(uid, transaction);

        var settings = item.ToSettings();

        return settings;
    }

    public async Task<MetaObjectStorable> GetItemByNameAsync(string name, IDbTransaction? transaction)
    {
        _query = SelectBuilder.Make()
          .From(_config.TableName)
          .Select("*")
          .WhereAnd("name = @name")
          .Parameter("name", name)
          .Query(_sqlDialect);

        var result = await _dbConnection.QueryFirstOrDefaultAsync<MetaObjectStorable>(_query.Text, _query.DynamicParameters, transaction);

        return result;
    }
}