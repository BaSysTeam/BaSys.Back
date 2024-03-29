using NUnit.Framework;
using BaSys.FluentQueries.QueryBuilders;
using BaSys.FluentQueries.Enums;
using System;

namespace BaSys.FluentQueries.UnitTests
{
    [TestFixture]
    public class TableExistsBuilderTests
    {
        [Test]
        public void Table_SetsTableName_ReturnsSelf()
        {
            var builder = TableExistsBuilder.Make();
            var result = builder.Table("TestTable");

            Assert.That(result, Is.EqualTo(builder), "Table method should return the TableExistsBuilder instance itself.");
        }

        [TestCase(SqlDialectKinds.MsSql, "IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'testtable')SELECT 1 AS Exists ELSE SELECT 0 AS Exists")]
        [TestCase(SqlDialectKinds.PgSql, "SELECT CASE WHEN EXISTS (SELECT FROM information_schema.tables WHERE table_type = 'BASE TABLE' AND table_name = 'testtable') THEN 1 ELSE 0 END AS Exists;")]
        public void Query_WithValidTableNameAndDialect_GeneratesCorrectQuery(SqlDialectKinds dialectKind, string expectedQueryText)
        {
            var builder = TableExistsBuilder.Make().Table("TestTable");
            var query = builder.Query(dialectKind);

            Assert.That(query.Text, Is.EqualTo(expectedQueryText), $"Query text does not match the expected format for {dialectKind}.");
        }

        [Test]
        public void Query_WithUnsupportedDialect_ThrowsNotImplementedException()
        {
            var builder = TableExistsBuilder.Make().Table("TestTable");

            // Assuming you have an unsupported dialect in your enum for testing
            Assert.Throws<NotImplementedException>(() => builder.Query((SqlDialectKinds)999), "Query method should throw NotImplementedException for unsupported SQL dialects.");
        }

        [Test]
        public void Query_WithEmptyTableName_ThrowsArgumentException()
        {
            var builder = TableExistsBuilder.Make();

            Assert.Throws<ArgumentException>(() => builder.Query(SqlDialectKinds.MsSql), "Query method should throw ArgumentException if the table name is not set.");
        }
    }
}
