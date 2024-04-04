using BaSys.Common.Enums;
using BaSys.FluentQueries.IntegrationTests.Infrastructure;
using BaSys.Host.DAL;
using BaSys.Host.DAL.TableManagers;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.FluentQueries.IntegrationTests
{
    [TestFixture]
    public class MetadataGroupTableManagerTests
    {
        private ConnectionStringService _connectionStringService;

        [SetUp]
        public void Setup()
        {
            _connectionStringService = new ConnectionStringService();   
        }

        [TearDown]
        public async Task Teardown()
        {
            string pgConnectionString = _connectionStringService.PgConnectionString;
            string msConnectionString = _connectionStringService.MsConnectionString;

            var factory = new BaSysConnectionFactory();

            using (IDbConnection connection = factory.CreateConnection(pgConnectionString, DbKinds.PgSql))
            {
                var manager = new MetadataGroupManager(connection);
                await manager.DropTableAsync();
            }

            using (IDbConnection connection = factory.CreateConnection(msConnectionString, DbKinds.MsSql))
            {
                var manager = new MetadataGroupManager(connection);
                await manager.DropTableAsync();
            }

        }

        [TestCase("pg_test_base")]
        [TestCase("ms_test_base")]
        public void GetConnectionStrings(string connectionStringName)
        {
            // Retrieve the connection string
            string connectionString = _connectionStringService.GetConnectionString(connectionStringName) ?? string.Empty;

            Console.WriteLine($"{connectionStringName}:{connectionString}");

            Assert.IsNotNull(connectionString);
        }

        [TestCase("pg_test_base")]
        [TestCase("ms_test_base")]
        public async Task CreateTable_MetadataGroup(string connectionStringName)
        {

            var tableExists = false;

            var factory = new BaSysConnectionFactory();

            var dbInfoRecord = _connectionStringService.GetDbInfoRecord(connectionStringName);

            using(IDbConnection connection = factory.CreateConnection(dbInfoRecord.ConnectionString, dbInfoRecord.DbKind))
            {
                var manager = new MetadataGroupManager(connection);
                await manager.CreateTableAsync();

                tableExists = await manager.TableExistsAsync();
            }

            Assert.IsTrue(tableExists); 
        }
    }
}
