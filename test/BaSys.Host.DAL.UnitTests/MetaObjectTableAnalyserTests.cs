using BaSys.Host.DAL.Helpers;
using BaSys.Host.DAL.TableChangeAnalyse;
using BaSys.Metadata.Models;

namespace BaSys.Host.DAL.UnitTests
{
    [TestFixture]
    public class MetaObjectTableAnalyserTests
    {
        [Test]
        public void Analyse_AddColumn_Commands()
        {
            var tableUid = Guid.NewGuid();
            var tableBefore = new MetaObjectTable()
            {
                Uid = tableUid,
                Title = "Header",
                Name = "header"
            };

            var tableAfter = new MetaObjectTable()
            {
                Uid = tableUid,
                Title = "Header",
                Name = "header"
            };

            var idColumn = new MetaObjectTableColumn(DataTypeDefaults.Int.Uid, true)
            {
                Uid = Guid.NewGuid(),
                Title = "Id",
                Name = "id",
            };

            var titleColumn = new MetaObjectTableColumn(DataTypeDefaults.String.Uid)
            {
                Uid = Guid.NewGuid(),
                Title = "Title",
                Name = "title",
            };
            titleColumn.DataSettings.Required = true;
            titleColumn.DataSettings.StringLength = 100;

            tableBefore.Columns.Add(idColumn);
            tableAfter.Columns.Add(idColumn);
            tableAfter.Columns.Add(titleColumn);
            
            var analyser = new MetaObjectTableChangeAnalyser(tableBefore, tableAfter);
            analyser.Analyze();

            Assert.That(analyser.Commands.Count, Is.EqualTo(1));
            Assert.That(analyser.Commands[0] is MetaObjectTableAddColumnCommand, Is.True);
            Assert.That(((MetaObjectTableAddColumnCommand)analyser.Commands[0]).Column.Name, Is.EqualTo("title"));

        }
    }
}
