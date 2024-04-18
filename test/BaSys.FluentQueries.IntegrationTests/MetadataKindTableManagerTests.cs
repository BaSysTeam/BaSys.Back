using BaSys.Common.Enums;
using BaSys.FluentQueries.IntegrationTests.Infrastructure;
using BaSys.Host.DAL.TableManagers;
using BaSys.Host.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.FluentQueries.IntegrationTests
{
    [TestFixture]
    public class MetadataKindTableManagerTests
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
                var manager = new MetadataKindManager(connection);
                await manager.DropTableAsync();
            }

            using (IDbConnection connection = factory.CreateConnection(msConnectionString, DbKinds.MsSql))
            {
                var manager = new MetadataKindManager(connection);
                await manager.DropTableAsync();
            }

        }

        [TestCase("pg_test_base")]
        [TestCase("ms_test_base")]
        public async Task CreateTable_MetadataKind(string connectionStringName)
        {

            var tableExists = false;

            var factory = new BaSysConnectionFactory();

            var dbInfoRecord = _connectionStringService.GetDbInfoRecord(connectionStringName);

            using (IDbConnection connection = factory.CreateConnection(dbInfoRecord.ConnectionString, dbInfoRecord.DbKind))
            {
                var manager = new MetadataKindManager(connection);
                await manager.CreateTableAsync();

                tableExists = await manager.TableExistsAsync();
            }

            Assert.IsTrue(tableExists);

        }

    }
}
