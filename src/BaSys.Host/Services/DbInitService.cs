using BaSys.Core.Abstractions;
using BaSys.DAL.Models.Admin;
using BaSys.Host.Abstractions;
using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.DataProviders;
using BaSys.Host.DAL.TableManagers;
using BaSys.SuperAdmin.Infrastructure.Models;
using System.Data;
using System.Diagnostics;
using System.Globalization;

namespace BaSys.Host.Services
{
    /// <summary>
    /// Creates required tables and fill neccessary data when DB created. Use Dapper.
    /// </summary>
    public sealed class DbInitService : IDbInitService
    {
        private readonly InitAppSettings? _initAppSettings;

        private readonly IMetaObjectKindsService _kindsService;
        private readonly IUserSettingsService _userService;
        private IDbConnection? _connection;

        public DbInitService(IConfiguration configuration, IMetaObjectKindsService kindsService, IUserSettingsService userService)
        {
            _initAppSettings = configuration.GetSection("InitAppSettings").Get<InitAppSettings>();
            if (_initAppSettings == null)
                throw new ApplicationException("InitAppSettings is not set in the config!");

            _kindsService = kindsService;
            _userService = userService;
        }

        public void SetUp(IDbConnection connection)
        {
            _connection = connection;
            _kindsService.SetUp(_connection);
            _userService.SetUp(_connection);
        }

        public async Task ExecuteAsync()
        {
            if (_connection == null)
                throw new ArgumentException($"Db connection is not setted up.");

            // Set language.
            var culture = _initAppSettings?.MainDb?.Culture ?? "en";
            SetCulture(culture);

            var tableManagers = new List<TableManagerBase>
            {
                new AppConstantsManager(_connection),
                new LoggerConfigManager(_connection),
                new MigrationManager(_connection),
                new MetaObjectKindManager(_connection),
                new UserSettingsManager(_connection),
                new FileStorageConfigManager(_connection),
                new UserGroupManager(_connection),
                new UserGroupUserManager(_connection),
                new UserGroupRoleManager(_connection),
                new UserGroupRightManager(_connection),
            };

            foreach (var tableManager in tableManagers)
                await CreateTableAsync(tableManager);

            await CheckTablesAsync();

            // Set user language in settings.
            var userName = _initAppSettings?.MainDb?.AdminLogin;
            if (!string.IsNullOrEmpty(userName))
            {
                await _userService.SetLanguageAsync(userName, culture);
            }

            // Fill metaobject kinds by default.
            await _kindsService.InsertStandardItemsAsync();

        }

        private async Task<int> CreateTableAsync(ITableManager tableManager)
        {
            var createdCount = 0;

            if (!await tableManager.TableExistsAsync())
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

        private async Task CheckTablesAsync()
        {
            await CheckAppConstantsAsync();
            await CheckLoggerConfigAsync();
        }

        private async Task CheckAppConstantsAsync()
        {
            var provider = new AppConstantsProvider(_connection);
            var collection = await provider.GetCollectionAsync(null);
            var appConstants = collection.FirstOrDefault();
            if (appConstants != null)
                return;

            appConstants = new AppConstants
            {
                Uid = Guid.NewGuid(),
                DataBaseUid = Guid.NewGuid(),
                ApplicationTitle = string.Empty
            };

            await provider.InsertAsync(appConstants, null);
        }

        private async Task CheckLoggerConfigAsync()
        {
            // var provider = new LoggerConfigProvider(_connection);
            // var collection = await provider.GetCollectionAsync(null);
            // var loggerConfig = collection.FirstOrDefault();
            // if (loggerConfig != null)
            //     return;
            //
            // loggerConfig = new LoggerConfig
            // {
            //     Uid = Guid.NewGuid(),
            //     MinimumLogLevel = EventTypeLevels.Info
            // };
            //
            // await provider.InsertAsync(loggerConfig, null);
        }

        private void SetCulture(string culture)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(culture);
            Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture(culture);
        }
    }
}
