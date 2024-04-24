using BaSys.Admin.Abstractions;
using BaSys.Common.Enums;
using BaSys.DAL.Models.Admin;
using BaSys.DAL.Models.Logging;
using BaSys.Host.Abstractions;
using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.DataProviders;
using BaSys.Host.DAL.TableManagers;
using BaSys.Metadata.Models;
using BaSys.SuperAdmin.Infrastructure.Models;
using System.Configuration;
using System.Data;
using System.Diagnostics;

namespace BaSys.Host.Services
{
    /// <summary>
    /// Creates required tables and fill neccessary data when DB created. Use Dapper.
    /// </summary>
    public sealed class DbInitService : IDbInitService
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
                new MetadataTreeNodeManager(_connection),
                new AppConstantsManager(_connection),
                new LoggerConfigManager(_connection),
                new MigrationManager(_connection),
                new LoggerConfigManager(_connection),
                new MetadataKindManager(_connection),
                new UserSettingsManager(_connection)
            };

            foreach (var tableManager in tableManagers)
                await CreateTableAsync(tableManager);

            await CheckTablesAsync();
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
            await CheckMetadataTreeNodesAsync();
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
            var provider = new LoggerConfigProvider(_connection);
            var collection = await provider.GetCollectionAsync(null);
            var loggerConfig = collection.FirstOrDefault();
            if (loggerConfig != null)
                return;

            loggerConfig = new LoggerConfig
            {
                Uid = Guid.NewGuid(),
                MinimumLogLevel = EventTypeLevels.Info
            };

            await provider.InsertAsync(loggerConfig, null);
        }

        private async Task CheckMetadataTreeNodesAsync()
        {
            var provider = new MetadataTreeNodesProvider(_connection);
            var standardNodes = await provider.GetStandardNodesAsync(null);
            if (standardNodes != null && standardNodes.Any())
                return;

            standardNodes = new List<MetadataTreeNode>
            {
                new MetadataTreeNode
                {
                    IsGroup = true,
                    IsStandard = true,
                    Title = "Metadata",
                    Uid = new Guid("60738680-DAFD-42C0-8923-585FC7985176")
                },
                new MetadataTreeNode
                {
                    IsGroup = true,
                    IsStandard = true,
                    Title = "System",
                    Uid = new Guid("AE28B333-3F36-4FEC-A276-92FCCC9B435C")
                }
            };

            foreach (var standardNode in standardNodes)
                await provider.InsertAsync(standardNode, null);

            standardNodes = await provider.GetCollectionAsync(null);
            var systemNode = standardNodes.FirstOrDefault(x => x.Title.ToLower() == "system");
            if (systemNode == null) 
                return;

            standardNodes = new List<MetadataTreeNode>
            {
                new MetadataTreeNode
                {
                    IsGroup = false,
                    IsStandard = true,
                    Title = "DataTypes",
                    Uid = new Guid("416C4B6C-48F7-426C-AA5A-774717C9984E"),
                    ParentUid = systemNode.Uid
                },
                new MetadataTreeNode
                {
                    IsGroup = false,
                    IsStandard = true,
                    Title = "MetadataKinds",
                    Uid = new Guid("CB930422-E50A-4C14-942F-B45DF8C23DE0"),
                    ParentUid = systemNode.Uid
                }
            };

            foreach (var standardNode in standardNodes)
                await provider.InsertAsync(standardNode, null);
        }
    }
}
