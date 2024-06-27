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
    public class InsertBuilderTests
    {
        [Test]
        public void InsertBuilder_OneRowExample_Query()
        {
            var builder = InsertBuilder.Make()
                .Table("metadata_groups")
                .Column("uid").Column("parentuid")
                .Value("uid").Value("parentuid");

            var msSqlQuery = builder.Query(SqlDialectKinds.MsSql);
            var pgSqlQuery = builder.Query(SqlDialectKinds.PgSql);

            Console.WriteLine("MS SQL:");
            Console.WriteLine(msSqlQuery.Text);

            Console.WriteLine("===================");
            Console.WriteLine("PG SQL:");
            Console.WriteLine(pgSqlQuery.Text);

            Assert.Pass();
        }

        [Test]
        public void InsertBuilder_OneRowExampleReturnId_Query()
        {
            var builder = InsertBuilder.Make()
                .Table("cat_currency").PrimaryKeyName("id").ReturnId(true)
                .Column("name").Column("code").Column("title").FillValuesByColumnNames(true);
               

            var msSqlQuery = builder.Query(SqlDialectKinds.MsSql);
            var pgSqlQuery = builder.Query(SqlDialectKinds.PgSql);

            Console.WriteLine("MS SQL:");
            Console.WriteLine(msSqlQuery.Text);

            Console.WriteLine("===================");
            Console.WriteLine("PG SQL:");
            Console.WriteLine(pgSqlQuery.Text);

            Assert.Pass();
        }

        [Test]
        public void InsertBuilder_FillValuesByColumnNames_Query()
        {
            var builder = InsertBuilder.Make()
                .Table("metadata_groups")
                .Column("uid").Column("parentuid")
                .FillValuesByColumnNames(true);

            var msSqlQuery = builder.Query(SqlDialectKinds.MsSql);
            var pgSqlQuery = builder.Query(SqlDialectKinds.PgSql);

            Console.WriteLine("MS SQL:");
            Console.WriteLine(msSqlQuery.Text);

            Console.WriteLine("===================");
            Console.WriteLine("PG SQL:");
            Console.WriteLine(pgSqlQuery.Text);

            Assert.Pass();
        }
    }
}
