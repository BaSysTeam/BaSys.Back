using BaSys.Common.Enums;
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
    public class CreateTableBuilderTests
    {
        private IConfigurationRoot _configuration;

        [SetUp]
        public void Setup()
        {
            // Build configuration
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory()) // Set the base path to the current directory
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            _configuration = builder.Build();
        }

        [TearDown]
        public async Task Teardown()
        {
            string pgConnectionString = _configuration?.GetConnectionString("pg_test_base");
            string msConnectionString = _configuration?.GetConnectionString("ms_test_base");

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
            string connectionString = _configuration?.GetConnectionString(connectionStringName) ?? string.Empty;

            Console.WriteLine($"{connectionStringName}:{connectionString}");

            Assert.IsNotNull(connectionString);
        }

        [TestCase("pg_test_base", DbKinds.PgSql)]
        [TestCase("ms_test_base", DbKinds.MsSql)]
        public async Task CreateTable_MetadataGroup(string connectionStringName, DbKinds dbKind)
        {

            var tableExists = false;

            var factory = new BaSysConnectionFactory();

            string connectionString = _configuration?.GetConnectionString(connectionStringName) ?? string.Empty;

            using(IDbConnection connection = factory.CreateConnection(connectionString, dbKind))
            {
                var manager = new MetadataGroupManager(connection);
                await manager.CreateTableAsync();

                tableExists = await manager.TableExistsAsync();
            }

            Assert.IsTrue(tableExists); 
        }
    }
}
