using BaSys.FluentQueries.Abstractions;
using BaSys.FluentQueries.QueryBuilders;
using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.ModelConfigurations;
using BaSys.Metadata.Models;
using BaSys.Metadata.Validators;
using Dapper;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.Host.DAL.DataProviders
{
    public sealed class MetaObjectKindsProvider : SystemObjectProviderBase<MetaObjectKind>
    {
        public MetaObjectKindsProvider(IDbConnection connection) : base(connection, new MetaObjectKindConfiguration())
        {
        }

        public override async Task<int> InsertAsync(MetaObjectKind item, IDbTransaction transaction)
        {
            _query = InsertBuilder.Make(_config)
           .FillValuesByColumnNames(true).Query(_sqlDialect);

            item.BeforeSave();
            var result = await _dbConnection.ExecuteAsync(_query.Text, item, transaction);

            return result;
        }

        public async Task<int> InsertSettingsAsync(MetaObjectKindSettings settings, IDbTransaction? transaction)
        {
            var item = new MetaObjectKind(settings);
            var result = await InsertAsync(item, transaction);

            return result;
        }

        public override async Task<int> UpdateAsync(MetaObjectKind item, IDbTransaction transaction)
        {
            var result = 0;

            _query = UpdateBuilder.Make(_config)
              .WhereAnd("uid = @uid")
              .Query(_sqlDialect);

            item.BeforeSave();
            result = await _dbConnection.ExecuteAsync(_query.Text, item, transaction);

            return result;
        }

        public async Task<MetaObjectKindSettings?> GetSettingsAsync(Guid uid, IDbTransaction? transaction = null)
        {
            var item = await GetItemAsync(uid, transaction);

            return item?.ToSettings();
        }

        public async Task<MetaObjectKindSettings?> GetSettingsByNameAsync(string name, IDbTransaction? transaction = null)
        {
            _query = SelectBuilder.Make()
              .From(_config.TableName)
              .Select("*")
              .WhereAnd("name = @name")
              .Parameter("name", name)
              .Query(_sqlDialect);

            var item = await _dbConnection.QueryFirstOrDefaultAsync<MetaObjectKind>(_query.Text, _query.DynamicParameters, transaction);

            return item?.ToSettings();

        }
    }
}
