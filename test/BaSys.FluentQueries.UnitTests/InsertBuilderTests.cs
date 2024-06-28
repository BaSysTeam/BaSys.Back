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
        [TestCase(SqlDialectKinds.MsSql, "InsertOneRowMsSql")]
        [TestCase(SqlDialectKinds.PgSql, "InsertOneRowPgSql")]
        public void InsertBuilder_OneRowExample_Query(SqlDialectKinds dialectKinds, string checkKey)
        {
            var builder = InsertBuilder.Make()
                .Table("metadata_groups")
                .Column("uid").Column("parentuid")
                .Value("uid").Value("parentuid");

            var query = builder.Query(dialectKinds);

            Console.WriteLine($"{dialectKinds}");
            Console.Write(query);

            var checkText = Texts.ResourceManager.GetString(checkKey);
            Assert.That(query.Text, Is.EqualTo(checkText));
        }

        [TestCase(SqlDialectKinds.MsSql, "InsertReturnIdMsSql")]
        [TestCase(SqlDialectKinds.PgSql, "InsertReturnIdPgSql")]

        public void InsertBuilder_OneRowExampleReturnId_Query(SqlDialectKinds dialectKinds, string checkKey)
        {
            var builder = InsertBuilder.Make()
                .Table("cat_currency").PrimaryKeyName("id").ReturnId(true)
                .Column("name").Column("code").Column("title").FillValuesByColumnNames(true);

            var query = builder.Query(dialectKinds);

            Console.WriteLine($"{dialectKinds}");
            Console.Write(query);

            var checkText = Texts.ResourceManager.GetString(checkKey);
            Assert.That(query.Text, Is.EqualTo(checkText));
        }

        [TestCase(SqlDialectKinds.MsSql, "InsertAutoFillValuesMsSql")]
        [TestCase(SqlDialectKinds.PgSql, "InsertAutoFillValuesPgSql")]
        public void InsertBuilder_FillValuesByColumnNames_Query(SqlDialectKinds dialectKinds, string checkKey)
        {
            var builder = InsertBuilder.Make()
                .Table("metadata_groups")
                .Column("uid").Column("parentuid")
                .FillValuesByColumnNames(true);

            var query = builder.Query(dialectKinds);

            Console.WriteLine($"{dialectKinds}");
            Console.Write(query);

            var checkText = Texts.ResourceManager.GetString(checkKey);
            Assert.That(query.Text, Is.EqualTo(checkText));
        }
    }
}
