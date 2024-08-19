using BaSys.Host.DAL.Helpers;
using BaSys.Host.DAL.TableChangeAnalyse;
using BaSys.Metadata.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            var idColumn = new MetaObjectTableColumn()
            {
                Uid = Guid.NewGuid(),
                Title = "Id",
                Name = "id",
                DataTypeUid = DataTypeDefaults.Int.Uid,
                PrimaryKey = true,
            };

            var titleColumn = new MetaObjectTableColumn()
            {
                Uid = Guid.NewGuid(),
                Title = "Title",
                Name = "title",
                DataTypeUid = DataTypeDefaults.String.Uid,
                Required = true,
                StringLength = 100
            };

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
