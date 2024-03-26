using BaSys.FluentQueries.Enums;
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
    public class CreateTableBuilderTests
    {
        [Test]
        public void CreateTableQuery()
        {
            var builder = CreateTableBuilder.Make()
                .Table("MetadataGroup")
                .PrimaryKey("Uid", DbType.Guid)
                .Column("ParentUid", DbType.Guid)
                .StringColumn("Title", 100)
                .StringColumn("IconClass", 20, false)
                .StringColumn("Memo", 300, false)
                .Column("IsStandard", DbType.Boolean, true);

            var msSqlQuery = builder.Query(DbKinds.MsSql);
            var pgSqlQuery = builder.Query(DbKinds.PgSql);

            Console.WriteLine("MS SQL:");
            Console.WriteLine(msSqlQuery.Text);
            Console.WriteLine("========================");

            Console.WriteLine("PG SQL:");
            Console.WriteLine(pgSqlQuery.Text);

            Assert.That(msSqlQuery.Text, Is.EqualTo(Texts.CreateTableMetadataGroupMsSql));
            Assert.That(pgSqlQuery.Text, Is.EqualTo(Texts.CreateTableMetadataGroupPgSql));  
           
        }
    }
}
