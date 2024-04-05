using BaSys.Admin.Abstractions;
using BaSys.Common.Models;
using BaSys.Host.Abstractions;
using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.DataProviders;
using BaSys.Host.DAL.TableManagers;
using BaSys.SuperAdmin.Infrastructure.Models;
using System.Configuration;
using System.Data;
using System.Diagnostics;

namespace BaSys.Host.Services
{
    /// <summary>
    /// Creates required tables and fill neccessary data when DB created. Use Dapper.
    /// </summary>
    public sealed class DbInitService: IDbInitService
    {
        private readonly InitAppSettings? _initAppSettings;
        private IDbConnection? _connection;

        public DbInitService(IConfiguration configuration)
        {
            _initAppSettings = configuration.GetSection("InitAppSettings").Get<InitAppSettings>();
            if (_initAppSettings == null)
                throw new ApplicationException("InitAppSettings is not set in the config!");
        }

        public void SetUp(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task ExecuteAsync()
        {
            if (_connection == null)
                throw new ArgumentException($"Db connection is not setted up.");

            var tableManagers = new List<TableManagerBase>
            {
                new MetadataGroupManager(_connection),
                new AppConstantsRecordManager(_connection)
            };

            foreach ( var tableManager in tableManagers )
                await CreateTableAsync(tableManager);


        }

        private async Task<int> CreateTableAsync(ITableManager tableManager)
        {
            var createdCount = 0;

            if (! await tableManager.TableExistsAsync())
            {
                try
                {
                    await tableManager.CreateTableAsync();

                    Debug.WriteLine($"Table created: {tableManager.TableName}.");
                   
                    createdCount++;
                  
                }
                catch (Exception ex)
                {
                    var message = $"Cannot create table {tableManager.TableName}. Message: {ex.Message}";
                    Debug.WriteLine(message);
                 
                }
            }
            else
            {
                Debug.WriteLine($"Table {tableManager.TableName} already exists");
            }

            return createdCount;
        }

        public async Task CheckTablesAsync()
        {
            await CheckAppConstantsAsync();
        }

        private async Task CheckAppConstantsAsync()
        {
            var provider = new AppConstantsRecordProvider(_connection);
            var collection = await provider.GetCollectionAsync(null);
            var appConstantsRecord = collection.FirstOrDefault();
            if (appConstantsRecord != null)
                return;
            
            var currentApp = _initAppSettings?.CurrentApp;
            if (currentApp == null)
                throw new ApplicationException("InitAppSettings:CurrentApp is not set in the config!");

            appConstantsRecord = new AppConstantsRecord
            {
                Uid = Guid.NewGuid(),
                DataBaseUid = Guid.NewGuid(),
                ApplicationTitle = currentApp.Title
            };

            await provider.InsertAsync(appConstantsRecord, null);
        }
    }
}
