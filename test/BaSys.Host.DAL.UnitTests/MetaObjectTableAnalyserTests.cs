using BaSys.Host.DAL.Helpers;
using BaSys.Host.DAL.TableChangeAnalyse;
using BaSys.Metadata.Models;

namespace BaSys.Host.DAL.UnitTests
{
    [TestFixture]
    public class MetaObjectTableAnalyserTests
    {
        private MetaObjectTable _headerBefore;
        private MetaObjectTable _headerAfter;
        private MetaObjectTableColumn _titleColumn;
        private MetaObjectTableColumn _memoColumn;

        [SetUp]
        public void SetUp()
        {
            var tableUid = Guid.NewGuid();
            _headerBefore = new MetaObjectTable()
            {
                Uid = tableUid,
                Title = "Header",
                Name = "header"
            };

            _headerAfter = new MetaObjectTable()
            {
                Uid = tableUid,
                Title = "Header",
                Name = "header"
            };

            var idColumn = new MetaObjectTableColumn(
                Guid.NewGuid(),
                "Id",
                "id",
                new MetaObjectTableColumnDataSettings
                {
                    DataTypeUid = DataTypeDefaults.Int.Uid,
                    PrimaryKey = true
                });

            _titleColumn = new MetaObjectTableColumn(
                Guid.NewGuid(),
                "Title",
                "title",
                new MetaObjectTableColumnDataSettings
                {
                    DataTypeUid = DataTypeDefaults.String.Uid,
                    Required = true,
                    StringLength = 100
                });

            _memoColumn = new MetaObjectTableColumn(
               Guid.NewGuid(),
               "Memo",
               "memo",
              new MetaObjectTableColumnDataSettings
              {
                  DataTypeUid = DataTypeDefaults.String.Uid,
                  StringLength = 300
              });


            _headerBefore.Columns.Add(idColumn);
            _headerBefore.Columns.Add(_titleColumn);

            _headerAfter.Columns.Add(idColumn);
            _headerAfter.Columns.Add(_titleColumn);
        }

        [Test]
        public void Analyse_AddColumn_Commands()
        {

            _headerAfter.Columns.Add(_memoColumn);

            var analyser = new MetaObjectTableChangeAnalyser(_headerBefore, _headerAfter);
            analyser.Analyze();

            Assert.That(analyser.Commands.Count, Is.EqualTo(1));
            Assert.That(analyser.Commands[0] is MetaObjectTableAddColumnCommand, Is.True);
            Assert.That(analyser.NeedAlterTable, Is.True);
            Assert.That(((MetaObjectTableAddColumnCommand)analyser.Commands[0]).Column.Name, Is.EqualTo("memo"));

        }

        [Test]
        public void Analyse_RenameColumn_AlterModel()
        {
            var memoColumnAfter = _memoColumn.Clone();
            memoColumnAfter.Name = "info";

            _headerBefore.Columns.Add(_memoColumn);
            _headerAfter.Columns.Add(memoColumnAfter);

            var analyser = new MetaObjectTableChangeAnalyser(_headerBefore, _headerAfter);
            analyser.Analyze();

            Assert.That(analyser.Commands.Count, Is.EqualTo(1));
            Assert.That(analyser.Commands[0] is MetaObjectTableRenameColumnCommand, Is.True);
            Assert.That(analyser.NeedAlterTable, Is.True);
            Assert.That(((MetaObjectTableRenameColumnCommand)analyser.Commands[0]).ColumnNameNew, Is.EqualTo("info"));

        }
    }
}
