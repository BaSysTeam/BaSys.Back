using BaSys.Common.Enums;
using BaSys.FluentQueries.Abstractions;
using BaSys.FluentQueries.Enums;
using BaSys.FluentQueries.IntegrationTests.Infrastructure;
using BaSys.FluentQueries.Models;
using BaSys.FluentQueries.QueryBuilders;
using BaSys.Host.DAL;
using Dapper;
using System.Data;

namespace BaSys.FluentQueries.IntegrationTests
{
    [TestFixture]
    public class AlterTableTests
    {
        private ConnectionStringService _connectionStringService;
        private string _tableName;
        private CreateTableBuilder _createBuilder;
        private TableColumn _rateColumn;
        private TableColumn _multiplierColumn;

        [SetUp]
        public void Setup()
        {
            _connectionStringService = new ConnectionStringService();
            _tableName = "cat_currency";

            _createBuilder = CreateTableBuilder.Make()
           .Table(_tableName)
           .PrimaryKey("id", DbType.Int32)
           .StringColumn("code", 3, true, true)
           .StringColumn("name", 3, true, true)
           .StringColumn("title", 100, true)
           .StringColumn("memo", 100, false);

            _rateColumn = new TableColumn()
            {
                Name = "rate",
                DbType = DbType.String,
                Required = true
            };
            _multiplierColumn = new TableColumn()
            {
                Name = "multiplier",
                DbType = DbType.String,
                Required = true
            };
        }

        [TearDown]
        public async Task Teardown()
        {
            string pgConnectionString = _connectionStringService.PgConnectionString;
            string msConnectionString = _connectionStringService.MsConnectionString;

            var factory = new BaSysConnectionFactory();

            var tablesToDelete = new List<string>
            {
                _tableName
            };

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
        public async Task AlterTable_AddColumn_ExecuteQuery(string connectionStringName)
        {

            var alterBuilder = AlterTableBuilder.Make()
                .Table(_tableName)
                .AddColumn(_rateColumn);

            var factory = new BaSysConnectionFactory();

            var dbInfoRecord = _connectionStringService.GetDbInfoRecord(connectionStringName);

            using (IDbConnection connection = factory.CreateConnection(dbInfoRecord.ConnectionString, dbInfoRecord.DbKind))
            {
                var dialect = (SqlDialectKinds)(int)dbInfoRecord.DbKind;
                var createQuery = _createBuilder.Query(dialect);
                var alterQuery = alterBuilder.Query(dialect);

                PrintQuery(dialect, alterQuery);

                await connection.ExecuteAsync(createQuery.Text);
                await connection.ExecuteAsync(alterQuery.Text);
            }

            Assert.Pass();
        }

        [TestCase("pg_test_base")]
        [TestCase("ms_test_base")]
        public async Task AlterTable_AddTwoColumns_ExecuteQuery(string connectionStringName)
        {

            var alterBuilder = AlterTableBuilder.Make()
                .Table(_tableName)
                .AddColumn(_rateColumn)
                .AddColumn(_multiplierColumn);

            var factory = new BaSysConnectionFactory();

            var dbInfoRecord = _connectionStringService.GetDbInfoRecord(connectionStringName);

            using (IDbConnection connection = factory.CreateConnection(dbInfoRecord.ConnectionString, dbInfoRecord.DbKind))
            {
                var dialect = (SqlDialectKinds)(int)dbInfoRecord.DbKind;
                var createQuery = _createBuilder.Query(dialect);
                var alterQuery = alterBuilder.Query(dialect);

                PrintQuery(dialect, alterQuery);

                await connection.ExecuteAsync(createQuery.Text);
                await connection.ExecuteAsync(alterQuery.Text);
            }

            Assert.Pass();
        }

        [TestCase("pg_test_base")]
        [TestCase("ms_test_base")]
        public async Task AlterTable_ChangeColumnName_ExecuteQuery(string connectionStringName)
        {

            var alterBuilder = AlterTableBuilder.Make()
                .Table(_tableName)
                .Rename("memo", "info");

            var factory = new BaSysConnectionFactory();

            var dbInfoRecord = _connectionStringService.GetDbInfoRecord(connectionStringName);

            using (IDbConnection connection = factory.CreateConnection(dbInfoRecord.ConnectionString, dbInfoRecord.DbKind))
            {
                var dialect = (SqlDialectKinds)(int)dbInfoRecord.DbKind;
                var createQuery = _createBuilder.Query(dialect);
                var alterQuery = alterBuilder.Query(dialect);

                PrintQuery(dialect, alterQuery);

                await connection.ExecuteAsync(createQuery.Text);
                await connection.ExecuteAsync(alterQuery.Text);
            }

            Assert.Pass();
        }

        [TestCase("pg_test_base")]
        [TestCase("ms_test_base")]
        public async Task AlterTable_DropTwoColumns_ExecuteQuery(string connectionStringName)
        {

            var alterBuilder = AlterTableBuilder.Make()
                .Table(_tableName)
                .DropColumn("title")
                .DropColumn("memo");

            var factory = new BaSysConnectionFactory();

            var dbInfoRecord = _connectionStringService.GetDbInfoRecord(connectionStringName);

            using (IDbConnection connection = factory.CreateConnection(dbInfoRecord.ConnectionString, dbInfoRecord.DbKind))
            {
                var dialect = (SqlDialectKinds)(int)dbInfoRecord.DbKind;
                var createQuery = _createBuilder.Query(dialect);
                var alterQuery = alterBuilder.Query(dialect);

                PrintQuery(dialect, alterQuery);

                await connection.ExecuteAsync(createQuery.Text);
                await connection.ExecuteAsync(alterQuery.Text);
            }

            Assert.Pass();
        }

        [TestCase("pg_test_base")]
        [TestCase("ms_test_base")]
        public async Task AlterTable_ChangeTwoColumnsName_ExecuteQuery(string connectionStringName)
        {

            var alterBuilder = AlterTableBuilder.Make()
                .Table(_tableName)
                .Rename("code","code_numeric")
                .Rename("memo", "info");

            var factory = new BaSysConnectionFactory();

            var dbInfoRecord = _connectionStringService.GetDbInfoRecord(connectionStringName);

            using (IDbConnection connection = factory.CreateConnection(dbInfoRecord.ConnectionString, dbInfoRecord.DbKind))
            {
                var dialect = (SqlDialectKinds)(int)dbInfoRecord.DbKind;
                var createQuery = _createBuilder.Query(dialect);
                var alterQuery = alterBuilder.Query(dialect);

                PrintQuery(dialect, alterQuery);

                await connection.ExecuteAsync(createQuery.Text);
                await connection.ExecuteAsync(alterQuery.Text);
            }

            Assert.Pass();
        }

        [TestCase("pg_test_base")]
        [TestCase("ms_test_base")]
        public async Task AlterTable_ChangeDataTypeOfColumn_ExecuteQuery(string connectionStringName)
        {

            var rateColumnBefore = _rateColumn.Clone();
            rateColumnBefore.DbType = DbType.Decimal;

            _createBuilder.Column(rateColumnBefore);

            var rateChangeModel = new ChangeColumnModel();
            rateChangeModel.Column = _rateColumn;
            rateChangeModel.DataTypeChanged = true;

            var alterBuilder = AlterTableBuilder.Make()
                .Table(_tableName)
                .ChangeColumn(rateChangeModel);

            var factory = new BaSysConnectionFactory();

            var dbInfoRecord = _connectionStringService.GetDbInfoRecord(connectionStringName);

            using (IDbConnection connection = factory.CreateConnection(dbInfoRecord.ConnectionString, dbInfoRecord.DbKind))
            {
                var dialect = (SqlDialectKinds)(int)dbInfoRecord.DbKind;
                var createQuery = _createBuilder.Query(dialect);
                var alterQuery = alterBuilder.Query(dialect);

                PrintQuery(dialect, alterQuery);

                await connection.ExecuteAsync(createQuery.Text);
                await connection.ExecuteAsync(alterQuery.Text);
            }

            Assert.Pass();
        }

        [TestCase("pg_test_base", true)]
        [TestCase("pg_test_base", false)]
        [TestCase("ms_test_base", true)]
        [TestCase("ms_test_base", false)]
        public async Task AlterTable_ChangeDataTypeAndRequiredOfColumn_ExecuteQuery(string connectionStringName, bool required)
        {

            var rateColumnBefore = _rateColumn.Clone();
            rateColumnBefore.DbType = DbType.Decimal;

            _createBuilder.Column(rateColumnBefore);

            var rateChangeModel = new ChangeColumnModel();
            rateChangeModel.Column = _rateColumn;
            rateChangeModel.Column.Required = required;
            rateChangeModel.DataTypeChanged = true;
            rateChangeModel.RequiredChanged = true;

            var alterBuilder = AlterTableBuilder.Make()
                .Table(_tableName)
                .ChangeColumn(rateChangeModel);

            var factory = new BaSysConnectionFactory();

            var dbInfoRecord = _connectionStringService.GetDbInfoRecord(connectionStringName);

            using (IDbConnection connection = factory.CreateConnection(dbInfoRecord.ConnectionString, dbInfoRecord.DbKind))
            {
                var dialect = (SqlDialectKinds)(int)dbInfoRecord.DbKind;
                var createQuery = _createBuilder.Query(dialect);
                var alterQuery = alterBuilder.Query(dialect);

                PrintQuery(dialect, alterQuery);

                await connection.ExecuteAsync(createQuery.Text);
                await connection.ExecuteAsync(alterQuery.Text);
            }

            Assert.Pass();
        }

        [TestCase("pg_test_base")]
        [TestCase("ms_test_base")]
        public async Task AlterTable_ChangeUniqueOfColumn_ExecuteQuery(string connectionStringName)
        {

            var rateColumnBefore = _rateColumn.Clone();
            rateColumnBefore.DbType = DbType.Decimal;

            _createBuilder.Column(rateColumnBefore);

            // Set unique.
            var rateColumn = _rateColumn.Clone();
            rateColumn.Unique = true;
            var rateChangeModel = new ChangeColumnModel(rateColumn, false, false, true);

            var addUniqueBuilder = AlterTableBuilder.Make()
                .Table(_tableName)
                .ChangeColumn(rateChangeModel);

            // Drop unique
            var rateColumnNotUnique = _rateColumn.Clone();
            rateColumnNotUnique.Unique = false;
            var rateNotUniqueChangeModel = new ChangeColumnModel(rateColumnNotUnique, false, false, true);

            var dropUniqueBuilder = AlterTableBuilder.Make()
                .Table(_tableName)
                .ChangeColumn(rateNotUniqueChangeModel);


            var factory = new BaSysConnectionFactory();

            var dbInfoRecord = _connectionStringService.GetDbInfoRecord(connectionStringName);

            using (IDbConnection connection = factory.CreateConnection(dbInfoRecord.ConnectionString, dbInfoRecord.DbKind))
            {
                var dialect = (SqlDialectKinds)(int)dbInfoRecord.DbKind;
                var createQuery = _createBuilder.Query(dialect);

                // Add uniquer constraint.
                var alterQuery = addUniqueBuilder.Query(dialect);

                // Drop uniquer constraint.
               var dropUniqueQuery = dropUniqueBuilder.Query(dialect);

                PrintQuery(dialect, alterQuery);
                PrintQuery(dialect, dropUniqueQuery);

                await connection.ExecuteAsync(createQuery.Text);
                await connection.ExecuteAsync(alterQuery.Text);
                await connection.ExecuteAsync(dropUniqueQuery.Text);
            }

            Assert.Pass();
        }

        [TestCase("pg_test_base")]
        [TestCase("ms_test_base")]
        public async Task AlterTable_ChangeDataTypeOfTwoColumns_ExecuteQuery(string connectionStringName)
        {

            var rateColumnBefore = _rateColumn.Clone();
            rateColumnBefore.DbType = DbType.Decimal;

            var multiplierColumnBefore = _multiplierColumn.Clone();
            multiplierColumnBefore.DbType = DbType.Decimal;

            _createBuilder.Column(rateColumnBefore).Column(multiplierColumnBefore);

            var rateChangeModel = new ChangeColumnModel();
            rateChangeModel.Column = _rateColumn;
            rateChangeModel.DataTypeChanged = true;

            var multiplierChangeModel = new ChangeColumnModel();
            multiplierChangeModel.Column = _multiplierColumn;
            multiplierChangeModel.DataTypeChanged = true;

            var alterBuilder = AlterTableBuilder.Make()
                .Table(_tableName)
                .ChangeColumn(rateChangeModel)
                .ChangeColumn(multiplierChangeModel);

            var factory = new BaSysConnectionFactory();

            var dbInfoRecord = _connectionStringService.GetDbInfoRecord(connectionStringName);

            using (IDbConnection connection = factory.CreateConnection(dbInfoRecord.ConnectionString, dbInfoRecord.DbKind))
            {
                var dialect = (SqlDialectKinds)(int)dbInfoRecord.DbKind;
                var createQuery = _createBuilder.Query(dialect);
                var alterQuery = alterBuilder.Query(dialect);

                PrintQuery(dialect, alterQuery);

                await connection.ExecuteAsync(createQuery.Text);
                await connection.ExecuteAsync(alterQuery.Text);
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
