using BaSys.FluentQueries.QueryBuilders;
using BaSys.Host.Abstractions;
using BaSys.Host.DAL;
using BaSys.Host.DAL.Helpers;
using BaSys.SuperAdmin.DAL;
using BaSys.SuperAdmin.DAL.Models;
using BaSys.SuperAdmin.Infrastructure.Models;
using Dapper;
using Microsoft.EntityFrameworkCore;
using System.Configuration;
using System.Linq.Dynamic.Core;

namespace BaSys.Host.Services
{
    public sealed class AppSettingsReader: IAppSettingsReader
    {
        private readonly IConfiguration _configuration;

        public AppSettingsReader(IConfiguration configuration)
        {
            _configuration = configuration;
          
        }

        public async Task<AppRecord> GetSettingsAsync()
        {
            var record = new AppRecord();

            var initAppSettings = _configuration.GetSection("InitAppSettings").Get<InitAppSettings>();
            if (initAppSettings == null)
            {
                return record;
            }

            var currentApp = initAppSettings?.CurrentApp;
            if (currentApp == null)
            {
                return record;
            }

          

            var saSection = initAppSettings?.Sa;
            if (saSection == null)
            {
                return record;
            }

            var appName = currentApp.Id;
            var connectionString = saSection.ConnectionString;
            var dbKind = saSection.DbKind.Value;
           

            var connectionFactory = new BaSysConnectionFactory();
            using(var connection = connectionFactory.CreateConnection(connectionString, dbKind))
            {
                var dialectKind = SqlDialectKindHelper.GetDialectKind(connection);

                var query = SelectBuilder.Make()
                    .From("AppRecords")
                    .Select("*").Query(dialectKind);

                var collection = await connection.QueryAsync<AppRecord>(query.Text);

                record = collection.FirstOrDefault(x => x.Id.Equals(appName, StringComparison.OrdinalIgnoreCase));
            }

            
            if(record == null)
            {
                record = new AppRecord();
            }
                

            return record;
        }
    }
}
