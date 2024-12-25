using BaSys.FluentQueries.Abstractions;
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
        private TableColumn _codeColumn;
        private TableColumn _titleColumn;

        [SetUp]
        public void SetUp()
        {
            _codeColumn = new TableColumn()
            {
                Name = "code",
                DbType = DbType.String,
                StringLength = 3,
                Required = true,
            };

            _titleColumn = new TableColumn()
            {
                Name = "title",
                DbType = DbType.String,
                StringLength = 100,
                Required = true,
            };
        }

        [TestCase(SqlDialectKinds.MsSql, "AlterTableAddOneColumnMsSQl")]
        [TestCase(SqlDialectKinds.PgSql, "AlterTableAddOneColumnPgSQl")]
        public void AlterTable_AddOneColumn_Query(SqlDialectKinds dialectKind, string checkKey)
        {
         

            var builder = AlterTableBuilder.Make()
                .Table("cat_currency")
                .AddColumn(_codeColumn);

            var query = builder.Query(dialectKind);

            PrintQuery(dialectKind, query);

            var checkText = Texts.ResourceManager.GetString(checkKey);
            Assert.That(query.Text, Is.EqualTo(checkText));

        }

        [TestCase(SqlDialectKinds.MsSql, "AlterTableAddTwoColumnsMsSQl")]
        [TestCase(SqlDialectKinds.PgSql, "AlterTableAddTwoColumnsPgSQl")]
        public void AlterTable_AddTwoColumns_Query(SqlDialectKinds dialectKind, string checkKey)
        {

            var builder = AlterTableBuilder.Make()
                .Table("cat_currency")
                .AddColumn(_codeColumn)
                .AddColumn(_titleColumn);

            var query = builder.Query(dialectKind);

            PrintQuery(dialectKind, query);

            var checkText = Texts.ResourceManager.GetString(checkKey);
            Assert.That(query.Text, Is.EqualTo(checkText));

        }

        [TestCase(SqlDialectKinds.MsSql, "AlterTableDropOneColumnMsSQl")]
        [TestCase(SqlDialectKinds.PgSql, "AlterTableDropOneColumnPgSQl")]
        public void AlterTable_DropOneColumn_Query(SqlDialectKinds dialectKind, string checkKey)
        {

            var builder = AlterTableBuilder.Make()
                .Table("cat_currency")
                .DropColumn("code");

            var query = builder.Query(dialectKind);

            PrintQuery(dialectKind, query);

            var checkText = Texts.ResourceManager.GetString(checkKey);
            Assert.That(query.Text, Is.EqualTo(checkText));

        }

        [TestCase(SqlDialectKinds.MsSql, "AlterTableDropTwoColumnsMsSQl")]
        [TestCase(SqlDialectKinds.PgSql, "AlterTableDropTwoColumnsPgSQl")]
        public void AlterTable_DropTwoColumns_Query(SqlDialectKinds dialectKind, string checkKey)
        {

            var builder = AlterTableBuilder.Make()
                .Table("cat_currency")
                .DropColumn("code")
                .DropColumn("title");

            var query = builder.Query(dialectKind);

            PrintQuery(dialectKind, query);

            var checkText = Texts.ResourceManager.GetString(checkKey);
            Assert.That(query.Text, Is.EqualTo(checkText));

        }

        [TestCase(SqlDialectKinds.MsSql, "AlterTableRenameColumnMsSQl")]
        [TestCase(SqlDialectKinds.PgSql, "AlterTableRenameColumnPgSQl")]
        public void AlterTable_RenameColumn_Query(SqlDialectKinds dialectKind, string checkKey)
        {
            var model = new AlterTableModel();
            model.TableName = "cat_currency";
            model.RenamedColumns.Add(new RenameColumnModel
            {
                OldName = "title",
                NewName = "description"
            });

            var builder = AlterTableBuilder.Make(model);

            var query = builder.Query(dialectKind);

            PrintQuery(dialectKind, query);

            var checkText = Texts.ResourceManager.GetString(checkKey);
            Assert.That(query.Text, Is.EqualTo(checkText));
        }

        private void PrintQuery(SqlDialectKinds dialectKind, IQuery query)
        {
            Console.WriteLine(dialectKind);
            Console.WriteLine(query);
            Console.WriteLine("==============");
        }
    }
}
