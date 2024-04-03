using BaSys.Host.Abstractions;
using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.TableManagers;
using System.Data;
using System.Diagnostics;

namespace BaSys.Host.Services
{
    /// <summary>
    /// Creates required tables and fill neccessary data when DB created. Use Dapper.
    /// </summary>
    public sealed class DbInitService: IDbInitService
    {
        private IDbConnection? _connection;

        public void SetUp(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task ExecuteAsync()
        {
            if (_connection == null)
                throw new ArgumentException($"Db connection is not setted up.");

            var metadataGroupManager = new MetadataGroupManager(_connection);
            await CreateTableAsync(metadataGroupManager);   
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
    }
}
