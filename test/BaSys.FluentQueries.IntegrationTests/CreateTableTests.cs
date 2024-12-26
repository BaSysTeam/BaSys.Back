using BaSys.Common.Enums;
using BaSys.FluentQueries.Abstractions;
using BaSys.FluentQueries.Enums;
using BaSys.FluentQueries.IntegrationTests.Infrastructure;
using BaSys.FluentQueries.QueryBuilders;
using BaSys.Host.DAL;
using Dapper;
using System.Data;

namespace BaSys.FluentQueries.IntegrationTests
{
    [TestFixture]
    internal class CreateTableTests
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

            var tablesToDelete = new List<string>();
            tablesToDelete.Add("int_key_example");
            tablesToDelete.Add("long_key_example");
            tablesToDelete.Add("guid_key_example");

            foreach (var tableName in tablesToDelete)
            {
                var builder = DropTableBuilder.Make().Table(tableName);

                using (IDbConnection connection = factory.CreateConnection(pgConnectionString, DbKinds.PgSql))
                {
                    var query = builder.Query(SqlDialectKinds.PgSql);
                    await connection.ExecuteAsync(query.Text);
                }

                using (IDbConnection connection = factory.CreateConnection(msConnectionString, DbKinds.MsSql))
                {
                    var query = builder.Query(SqlDialectKinds.MsSql);
                    await connection.ExecuteAsync(query.Text);
                }
            }

        }


        [TestCase("pg_test_base")]
        [TestCase("ms_test_base")]
        public async Task CreateTable_IntPrimaryKey_Table(string connectionStringName)
        {

            var builder = CreateTableBuilder.Make()
              .Table("int_key_example")
              .PrimaryKey("Id", DbType.Int32)
              .StringColumn("name", 100, false);

            var factory = new BaSysConnectionFactory();

            var dbInfoRecord = _connectionStringService.GetDbInfoRecord(connectionStringName);

            using (IDbConnection connection = factory.CreateConnection(dbInfoRecord.ConnectionString, dbInfoRecord.DbKind))
            {
                var dialect = (SqlDialectKinds)(int)dbInfoRecord.DbKind;
                var query = builder.Query(dialect);
                await connection.ExecuteAsync(query.Text);
            }

            Assert.Pass();
        }

        [TestCase("pg_test_base")]
        [TestCase("ms_test_base")]
        public async Task CreateTable_IntPrimaryKeyWhithUniqueColumn_Table(string connectionStringName)
        {

            var builder = CreateTableBuilder.Make()
              .Table("int_key_example")
              .PrimaryKey("Id", DbType.Int32)
              .StringColumn("name", 30, true, true);

            var factory = new BaSysConnectionFactory();

            var dbInfoRecord = _connectionStringService.GetDbInfoRecord(connectionStringName);

            using (IDbConnection connection = factory.CreateConnection(dbInfoRecord.ConnectionString, dbInfoRecord.DbKind))
            {
                var dialect = (SqlDialectKinds)(int)dbInfoRecord.DbKind;
                var query = builder.Query(dialect);

                PrintQuery(dialect, query);
                await connection.ExecuteAsync(query.Text);
            }

            Assert.Pass();
        }

        [TestCase("pg_test_base")]
        [TestCase("ms_test_base")]
        public async Task CreateTable_LongPrimaryKey_Table(string connectionStringName)
        {

            var builder = CreateTableBuilder.Make()
              .Table("long_key_example")
              .PrimaryKey("Id", DbType.Int64)
              .StringColumn("name", 100, false);

            var factory = new BaSysConnectionFactory();

            var dbInfoRecord = _connectionStringService.GetDbInfoRecord(connectionStringName);

            using (IDbConnection connection = factory.CreateConnection(dbInfoRecord.ConnectionString, dbInfoRecord.DbKind))
            {
                var dialect = (SqlDialectKinds)(int)dbInfoRecord.DbKind;
                var query = builder.Query(dialect);
                await connection.ExecuteAsync(query.Text);
            }

            Assert.Pass();
        }

        [TestCase("pg_test_base")]
        [TestCase("ms_test_base")]
        public async Task CreateTable_GuidPrimaryKey_Table(string connectionStringName)
        {

            var builder = CreateTableBuilder.Make()
              .Table("guid_key_example")
              .PrimaryKey("Id", DbType.Guid)
              .StringColumn("name", 100, false);

            var factory = new BaSysConnectionFactory();

            var dbInfoRecord = _connectionStringService.GetDbInfoRecord(connectionStringName);

            using (IDbConnection connection = factory.CreateConnection(dbInfoRecord.ConnectionString, dbInfoRecord.DbKind))
            {
                var dialect = (SqlDialectKinds)(int)dbInfoRecord.DbKind;
                var query = builder.Query(dialect);
                await connection.ExecuteAsync(query.Text);
            }

            Assert.Pass();
        }

        private void PrintQuery(SqlDialectKinds dialectKind, IQuery query)
        {
            Console.WriteLine(dialectKind);
            Console.WriteLine(query);
            Console.WriteLine("==============");
        }
    }
}
