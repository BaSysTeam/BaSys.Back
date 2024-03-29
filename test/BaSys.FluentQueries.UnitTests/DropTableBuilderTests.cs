using BaSys.FluentQueries.Enums;
using BaSys.FluentQueries.QueryBuilders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.FluentQueries.UnitTests
{
    [TestFixture]
    public class DropTableBuilderTests
    {

        [Test]
        public void Table_SetsTableName_ReturnsSelf()
        {
            var builder = DropTableBuilder.Make();
            var result = builder.Table("MyTable");

            Assert.That(result, Is.EqualTo(builder), "Table method should return the builder instance itself.");
        }

        [TestCase(SqlDialectKinds.MsSql, "DROP TABLE IF EXISTS MyTable;")]
        [TestCase(SqlDialectKinds.PgSql, "DROP TABLE IF EXISTS MyTable;")]
        public void Query_WithValidTableNameAndDialect_GeneratesCorrectQuery(SqlDialectKinds dbKind, string expectedQueryText)
        {
            var builder = DropTableBuilder.Make().Table("MyTable");
            var query = builder.Query(dbKind);

            Assert.That(query.Text, Is.EqualTo(expectedQueryText), $"Query text does not match the expected format for {dbKind}.");
        }

        [Test]
        public void Query_WithUnsupportedDialect_ThrowsNotImplementedException()
        {
            var builder = DropTableBuilder.Make().Table("MyTable");

            // Assuming you have an unsupported dialect in your enum for testing
            Assert.Throws<NotImplementedException>(() => builder.Query((SqlDialectKinds)999), "Query method should throw NotImplementedException for unsupported SQL dialects.");
        }

        [Test]
        public void Query_WithEmptyTableName_ThrowsArgumentException()
        {
            var builder = DropTableBuilder.Make();

            Assert.Throws<ArgumentException>(() => builder.Query(SqlDialectKinds.MsSql), "Query method should throw ArgumentException if the table name is not set.");
        }

    }
}
