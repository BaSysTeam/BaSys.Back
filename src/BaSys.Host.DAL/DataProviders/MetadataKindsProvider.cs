﻿using BaSys.FluentQueries.Abstractions;
using BaSys.FluentQueries.QueryBuilders;
using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.ModelConfigurations;
using BaSys.Metadata.Models;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.Host.DAL.DataProviders
{
    public sealed class MetadataKindsProvider : SystemObjectProviderBase<MetadataKind>
    {
        public MetadataKindsProvider(IDbConnection connection) : base(connection, new MetadataKindConfiguration())
        {
        }

        public override async Task<int> InsertAsync(MetadataKind item, IDbTransaction transaction)
        {
            _query = InsertBuilder.Make(_config)
           .FillValuesByColumnNames(true).Query(_sqlDialect);

            var result = await _dbConnection.ExecuteAsync(_query.Text, item, transaction);

            return result;
        }

        public async Task<int> InsertSettingsAsync(MetadataKindSettings settings, IDbTransaction? transaction)
        {
            var item = new MetadataKind(settings);
            var result = await InsertAsync(item, transaction);

            return result;
        }

        public override async Task<int> UpdateAsync(MetadataKind item, IDbTransaction transaction)
        {
            var result = 0;

            _query = UpdateBuilder.Make(_config)
              .WhereAnd("uid = @uid")
              .Query(_sqlDialect);

            result = await _dbConnection.ExecuteAsync(_query.Text, item, transaction);

            return result;
        }

        public async Task<MetadataKindSettings?> GetSettingsByNameAsync(string name, IDbTransaction? transaction = null)
        {
            _query = SelectBuilder.Make()
              .From(_config.TableName)
              .Select("*")
              .WhereAnd("name = @name")
              .Parameter("name", name)
              .Query(_sqlDialect);

            var item = await _dbConnection.QueryFirstOrDefaultAsync<MetadataKind>(_query.Text, _query.DynamicParameters, transaction);

            return item?.ToSettings();

        }
    }
}