using BaSys.FluentQueries.QueryBuilders;
using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.ModelConfigurations;
using BaSys.Metadata.Models;
using BaSys.Metadata.Models.MenuModel;
using Dapper;
using MessagePack;
using System.Data;

namespace BaSys.Host.DAL.DataProviders
{
    public class MetaObjectMenuProvider : SystemObjectProviderBase<MetaObjectMenu>
    {
        public MetaObjectMenuProvider(IDbConnection dbConnection, string kindName) : base(dbConnection, new MetadataObjectConfiguration(kindName))
        {

        }

        public override async Task<IEnumerable<MetaObjectMenu>> GetCollectionAsync(IDbTransaction transaction)
        {
            _query = SelectBuilder.Make().From(_config.TableName).Select("*").OrderBy("title").Query(_sqlDialect);

            var result = await _dbConnection.QueryAsync<MetaObjectMenu>(_query.Text, null, transaction);

            return result;
        }

        public override async Task<Guid> InsertAsync(MetaObjectMenu item, IDbTransaction transaction)
        {
            if (item.Uid == Guid.Empty)
            {
                item.Uid = Guid.NewGuid();
            }
            item.BeforeSave();

            _query = InsertBuilder.Make(_config)
                .FillValuesByColumnNames(true)
                .Query(_sqlDialect);

            var insertedCount = await _dbConnection.ExecuteAsync(_query.Text, item, transaction);

            return InsertedUid(insertedCount, item.Uid);
        }

        public override async Task<int> UpdateAsync(MetaObjectMenu item, IDbTransaction transaction)
        {
            item.BeforeSave();

            _query = UpdateBuilder.Make(_config)
           .WhereAnd("uid = @uid")
           .Query(_sqlDialect);

            var result = await _dbConnection.ExecuteAsync(_query.Text, item, transaction);

            return result;
        }


        /// <summary>
        /// Insert all colunms
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<int> InsertSettingsAsync(MetaObjectMenuSettings settings, IDbTransaction? transaction)
        {
            var settingsArr = MessagePackSerializer.Serialize(settings);
            var item = new MetaObjectMenu
            {
                Uid = settings.Uid,
                Name = settings.Name,
                Title = settings.Title,
                Memo = settings.Memo,
                MetaObjectKindUid = MetaObjectKindDefaults.Menu.Uid,
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
        public async Task<int> UpdateSettingsAsync(MetaObjectMenuSettings settings, IDbTransaction? transaction)
        {
            var settingsArr = MessagePackSerializer.Serialize(settings);
            var item = new MetaObjectMenu
            {
                Uid = settings.Uid,
                Name = settings.Name,
                Title = settings.Title,
                Memo = settings.Memo,
                MetaObjectKindUid = MetaObjectKindDefaults.Menu.Uid,
                IsActive = settings.IsActive,
                Version = settings.Version,
                SettingsStorage = settingsArr
            };
            var result = await UpdateAsync(item, transaction);

            return result;
        }

        public async Task<MetaObjectMenuSettings?> GetSettingsItemAsync(Guid uid, IDbTransaction? transaction)
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

        public async Task<MetaObjectMenu> GetItemByNameAsync(string name, IDbTransaction? transaction)
        {
            _query = SelectBuilder.Make()
              .From(_config.TableName)
              .Select("*")
              .WhereAnd("name = @name")
              .Parameter("name", name)
              .Query(_sqlDialect);

            var result = await _dbConnection.QueryFirstOrDefaultAsync<MetaObjectMenu>(_query.Text, _query.DynamicParameters, transaction);

            return result;
        }

    }
}
