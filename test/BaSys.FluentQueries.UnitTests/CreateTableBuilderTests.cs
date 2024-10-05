using BaSys.FluentQueries.Enums;
using BaSys.FluentQueries.QueryBuilders;
using BaSys.Host.DAL.ModelConfigurations;
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
        public void CreateTable_MetadataGroupByBuilder_Query()
        {
            var builder = CreateTableBuilder.Make()
                .Table("sys_metadata_groups")
                .PrimaryKey("Uid", DbType.Guid)
                .Column("ParentUid", DbType.Guid, false)
                .StringColumn("Title", 100, true)
                .StringColumn("IconClass", 20, false)
                .StringColumn("Memo", 300, false)
                .Column("IsStandard", DbType.Boolean, true);

            var msSqlQuery = builder.Query(SqlDialectKinds.MsSql);
            var pgSqlQuery = builder.Query(SqlDialectKinds.PgSql);

            Console.WriteLine("MS SQL:");
            Console.WriteLine(msSqlQuery.Text);
            Console.WriteLine("========================");

            Console.WriteLine("PG SQL:");
            Console.WriteLine(pgSqlQuery.Text);

            Assert.That(msSqlQuery.Text, Is.EqualTo(Texts.CreateTableMetadataGroupMsSql));
            Assert.That(pgSqlQuery.Text, Is.EqualTo(Texts.CreateTableMetadataGroupPgSql));

        }

        [Test]
        public void CreateTable_MetadataGroupByConfig_Query()
        {

            var config = new MetaObjectKindConfiguration();

            var builder = CreateTableBuilder.Make(config);

            var msSqlQuery = builder.Query(SqlDialectKinds.MsSql);
            var pgSqlQuery = builder.Query(SqlDialectKinds.PgSql);

            Console.WriteLine("MS SQL:");
            Console.WriteLine(msSqlQuery.Text);
            Console.WriteLine("========================");

            Console.WriteLine("PG SQL:");
            Console.WriteLine(pgSqlQuery.Text);

            Assert.That(msSqlQuery.Text, Is.EqualTo(Texts.CreateTableMetadataGroupMsSql));
            Assert.That(pgSqlQuery.Text, Is.EqualTo(Texts.CreateTableMetadataGroupPgSql));

        }

        [TestCase(SqlDialectKinds.MsSql, "CreateTableWithUniqueColumnMsSql")]
        [TestCase(SqlDialectKinds.PgSql, "CreateTableWithUniqueColumnPgSql")]
        public void CreateTable_UniqueColumn_Query(SqlDialectKinds dialectKinds, string checkKey)
        {
            var builder = CreateTableBuilder.Make().Table("sys_metadata_kinds")
                .PrimaryKey("uid", DbType.Guid)
                .StringColumn("title",100, true)
                .StringColumn("prefix", 4, true, true);

            var query = builder.Query(dialectKinds);

            Console.WriteLine($"{dialectKinds}");
            Console.Write(query);

            var checkText = Texts.ResourceManager.GetString(checkKey);
            Assert.That(query.Text, Is.EqualTo(checkText));
        }

        [Test]
        public void CreateTable_WithMultiplePrimaryKeys_ShouldThrow()
        {
            // Setup
            var builder = CreateTableBuilder.Make().Table("Users").PrimaryKey("Id1", DbType.Int32).PrimaryKey("Id2", DbType.Int32);

            // Exercise & Verify
            Assert.Throws<InvalidOperationException>(() => builder.Query(SqlDialectKinds.PgSql));
        }

        [Test]
        public void CreateTable_WithDoubleColumns_ShouldThrow()
        {
            // Setup
            var builder = CreateTableBuilder.Make().Table("Users")
                .PrimaryKey("Title", DbType.String)
                .PrimaryKey("Title", DbType.String);

            // Exercise & Verify
            Assert.Throws<InvalidOperationException>(() => builder.Query(SqlDialectKinds.PgSql));
        }

        [Test]
        public void CreateTable_WithEmptyTableName_ShouldThrow()
        {
            // Setup
            var builder = CreateTableBuilder.Make()
                .PrimaryKey("Title", DbType.String)
                .PrimaryKey("Title", DbType.String);

            // Exercise & Verify
            Assert.Throws<InvalidOperationException>(() => builder.Query(SqlDialectKinds.PgSql));
        }
    }
}
