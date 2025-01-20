using BaSys.FluentQueries.QueryBuilders;
using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.ModelConfigurations;
using BaSys.Metadata.Defaults;
using BaSys.Metadata.Models;
using BaSys.Metadata.Models.MenuModel;
using BaSys.Metadata.Models.WorkflowModel;
using Dapper;
using MessagePack;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.Host.DAL.DataProviders
{
    public class MetaWorkflowsProvider: SystemObjectProviderBase<MetaObjectWorkflow>
    {
        public const string KindName = "workflow";

        public MetaWorkflowsProvider(IDbConnection dbConnection) : base(dbConnection, new MetadataObjectConfiguration(KindName))
        {
            
        }

        public async Task<IEnumerable<MetaObjectWorkflow>> GetCollectionAsync(bool? isActive, IDbTransaction? transaction)
        {
            var builder = SelectBuilder.Make().From(_config.TableName).Select("*").OrderBy("title");

            if (isActive.HasValue)
            {
                builder.WhereAnd("isactive = @isActive").Parameter("isActive", isActive.Value, DbType.Boolean);
            }

            _query = builder.Query(_sqlDialect);

            var result = await _dbConnection.QueryAsync<MetaObjectWorkflow>(_query.Text, _query.DynamicParameters, transaction);

            return result;
        }

        public override async Task<Guid> InsertAsync(MetaObjectWorkflow item, IDbTransaction transaction)
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

        public override async Task<int> UpdateAsync(MetaObjectWorkflow item, IDbTransaction transaction)
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
        public async Task<int> InsertSettingsAsync(WorkflowSettings settings, IDbTransaction? transaction)
        {
            var settingsArr = MessagePackSerializer.Serialize(settings);
            var item = new MetaObjectWorkflow
            {
                Uid = settings.Uid,
                Name = settings.Name,
                Title = settings.Title,
                Memo = settings.Memo,
                MetaObjectKindUid = MetaObjectKindDefaults.Workflow.Uid,
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
        public async Task<int> UpdateSettingsAsync(WorkflowSettings settings, IDbTransaction? transaction)
        {
            var settingsArr = MessagePackSerializer.Serialize(settings);
            var item = new MetaObjectWorkflow
            {
                Uid = settings.Uid,
                Name = settings.Name,
                Title = settings.Title,
                Memo = settings.Memo,
                MetaObjectKindUid = MetaObjectKindDefaults.Workflow.Uid,
                IsActive = settings.IsActive,
                Version = settings.Version,
                SettingsStorage = settingsArr
            };
            var result = await UpdateAsync(item, transaction);

            return result;
        }

        public async Task<WorkflowSettings?> GetSettingsItemAsync(Guid uid, IDbTransaction? transaction)
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

        public async Task<MetaObjectWorkflow?> GetItemByNameAsync(string name, IDbTransaction? transaction)
        {
            _query = SelectBuilder.Make()
              .From(_config.TableName)
              .Select("*")
              .WhereAnd("name = @name")
              .Parameter("name", name)
              .Query(_sqlDialect);

            var result = await _dbConnection.QueryFirstOrDefaultAsync<MetaObjectWorkflow>(_query.Text, _query.DynamicParameters, transaction);

            return result;
        }
    }
}
