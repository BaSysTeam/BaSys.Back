using BaSys.FluentQueries.Enums;
using BaSys.FluentQueries.Models;
using BaSys.FluentQueries.QueryBuilders;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.FluentQueries.UnitTests
{
    [TestFixture]
    public class AlterTableBuilderTests
    {
        [TestCase(SqlDialectKinds.MsSql, "AlterTableAddColumnsMsSQl")]
        [TestCase(SqlDialectKinds.PgSql, "AlterTableAddColumnsPgSQl")]
        public void AlterTable_AddColumns_Query(SqlDialectKinds dialectKind, string checkKey)
        {
            var codeColumn = new TableColumn()
            {
                Name = "code",
                DbType = DbType.String,
                StringLength = 3,
                Required = true,
            };

            var titleColumn = new TableColumn()
            {
                Name = "title",
                DbType = DbType.String,
                StringLength = 100,
                Required = true,
            };

            var builder = AlterTableBuilder.Make()
                .Table("cat_currency")
                .AddColumn(codeColumn)
                .AddColumn(titleColumn);

            var query = builder.Query(dialectKind);

            Console.WriteLine(dialectKind);
            Console.WriteLine(query);
            Console.WriteLine("==============");

            Assert.Pass();

        }
    }
}
