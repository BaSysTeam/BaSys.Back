using BaSys.FluentQueries.Abstractions;
using BaSys.FluentQueries.Enums;
using BaSys.FluentQueries.Models;
using BaSys.FluentQueries.QueryBuilders;
using System.Data;

namespace BaSys.FluentQueries.UnitTests
{
    [TestFixture]
    public class AlterTableBuilderTests
    {
        private TableColumn _codeColumn;
        private TableColumn _titleColumn;
        private TableColumn _rateColumn;
        private TableColumn _multiplierColumn;

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

            _rateColumn = new TableColumn()
            {
                Name = "rate",
                DbType = DbType.String,
                StringLength = 100
            };

            _multiplierColumn = new TableColumn()
            {
                Name = "multiplier",
                DbType = DbType.Decimal,
                NumberDigits = 3,
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

        [TestCase(SqlDialectKinds.MsSql, "AlterTableChangeDataTypeOfColumnMsSQl")]
        [TestCase(SqlDialectKinds.PgSql, "AlterTableChangeDataTypeOfColumnPgSQl")]
        public void AlterTable_ChangeDataTypeOfColumn_Query(SqlDialectKinds dialectKind, string checkKey)
        {
            var model = new AlterTableModel();
            model.TableName = "cat_currency";

            var changeColumnModel = new ChangeColumnModel();
            changeColumnModel.Column = _rateColumn;
            changeColumnModel.DataTypeChanged = true;
            model.ChangedColumns.Add(changeColumnModel);
           

            var builder = AlterTableBuilder.Make(model);

            var query = builder.Query(dialectKind);

            PrintQuery(dialectKind, query);

            var checkText = Texts.ResourceManager.GetString(checkKey);
            Assert.That(query.Text, Is.EqualTo(checkText));
        }

        [TestCase(SqlDialectKinds.MsSql, "AlterTableChangeDataTypeAndRequiredTrueOfColumnMsSQl", true)]
        [TestCase(SqlDialectKinds.MsSql, "AlterTableChangeDataTypeAndRequiredFalseOfColumnMsSQl", false)]
        [TestCase(SqlDialectKinds.PgSql, "AlterTableChangeDataTypeAndRequiredTrueOfColumnPgSQl", true)]
        [TestCase(SqlDialectKinds.PgSql, "AlterTableChangeDataTypeAndRequiredFalseOfColumnPgSQl", false)]
        public void AlterTable_ChangeDataTypeAndRequiredOfColumn_Query(SqlDialectKinds dialectKind, string checkKey, bool required)
        {
            var model = new AlterTableModel();
            model.TableName = "cat_currency";

            var changeColumnModel = new ChangeColumnModel();
            changeColumnModel.Column = _rateColumn;
            changeColumnModel.Column.Required = required;
            changeColumnModel.DataTypeChanged = true;
            changeColumnModel.RequiredChanged = true;
            model.ChangedColumns.Add(changeColumnModel);


            var builder = AlterTableBuilder.Make(model);

            var query = builder.Query(dialectKind);

            PrintQuery(dialectKind, query);

            var checkText = Texts.ResourceManager.GetString(checkKey);
            Assert.That(query.Text, Is.EqualTo(checkText));
        }

        [TestCase(SqlDialectKinds.MsSql, "AlterTableChangeUniqueTrueOfColumnMsSQl", true)]
        [TestCase(SqlDialectKinds.MsSql, "AlterTableChangeUniqueFalseOfColumnMsSQl", false)]
        [TestCase(SqlDialectKinds.PgSql, "AlterTableChangeUniqueTrueOfColumnPgSQl", true)]
        [TestCase(SqlDialectKinds.PgSql, "AlterTableChangeUniqueFalseOfColumnPgSQl", false)]
        public void AlterTable_ChangeUniqueOfColumn_Query(SqlDialectKinds dialectKind, string checkKey, bool unique)
        {
            var model = new AlterTableModel();
            model.TableName = "cat_currency";

            var changeColumnModel = new ChangeColumnModel();
            changeColumnModel.Column = _rateColumn;
            changeColumnModel.Column.Unique = unique;
            changeColumnModel.UniqueChanged = true;
            model.ChangedColumns.Add(changeColumnModel);

            var builder = AlterTableBuilder.Make(model);

            var query = builder.Query(dialectKind);

            PrintQuery(dialectKind, query);

            var checkText = Texts.ResourceManager.GetString(checkKey);
            Assert.That(query.Text, Is.EqualTo(checkText));
        }

        [TestCase(SqlDialectKinds.MsSql, "AlterTableChangeDataTypeOfTwoColumnsMsSQl")]
        [TestCase(SqlDialectKinds.PgSql, "AlterTableChangeDataTypeOfTWoColumnsPgSQl")]
        public void AlterTable_ChangeDataTypeOfTwoColumns_Query(SqlDialectKinds dialectKind, string checkKey)
        {
            var model = new AlterTableModel();
            model.TableName = "cat_currency";

            var rateChangeModel = new ChangeColumnModel();
            rateChangeModel.Column = _rateColumn;
            rateChangeModel.DataTypeChanged = true;
            model.ChangedColumns.Add(rateChangeModel);

            var multiplierChangeModel = new ChangeColumnModel();
            multiplierChangeModel.Column = _multiplierColumn;
            multiplierChangeModel.DataTypeChanged = true;
            model.ChangedColumns.Add(multiplierChangeModel);

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
