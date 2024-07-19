using BaSys.Host.DAL.TableChangeAnalyse;
using BaSys.Metadata.Models;

namespace BaSys.Host.DAL.UnitTests
{
    [TestFixture]
    public class MetaObjectChangeAnalyserTests
    {
        private MetaObjectStorableSettings _settingsBefore;
        private MetaObjectStorableSettings _settingsAfter;
        private MetaObjectTable _productTable;
        private MetaObjectTable _infoTable;

        [SetUp]
        public void SetUp()
        {
            _settingsBefore = new MetaObjectStorableSettings();
            _settingsBefore.Uid = Guid.NewGuid();

            _settingsAfter = new MetaObjectStorableSettings();
            _settingsAfter.Uid = _settingsBefore.Uid;

            _productTable = new MetaObjectTable()
            {
                Uid = Guid.NewGuid(),
                Name = "products"
            };

            _infoTable = new MetaObjectTable()
            {
                Uid = Guid.NewGuid(),
                Name = "info"
            };
        }

        [Test]
        public void Analyse_TableCreated_CreateTableCommand()
        {
            _settingsAfter.DetailTables.Add(_productTable);

            var analyzer = new MetaObjectStorableChangeAnalyser(_settingsBefore, _settingsAfter);
            analyzer.Analyze();

            Assert.That(analyzer.Commands.Count, Is.EqualTo(1));
            Assert.IsInstanceOf<MetaObjectCreateTableCommand>(analyzer.Commands[0]);

        }

        [Test]
        public void Analyse_TableRemoved_DropTableCommand()
        {
            _settingsBefore.DetailTables.Add(_productTable);
            _settingsBefore.DetailTables.Add(_infoTable);

            _settingsAfter.DetailTables.Add(_infoTable);

            var analyzer = new MetaObjectStorableChangeAnalyser(_settingsBefore, _settingsAfter);
            analyzer.Analyze();

            Assert.That(analyzer.Commands.Count, Is.EqualTo(1));

            var dropCommand = analyzer.Commands[0];
            Assert.IsInstanceOf<MetaObjectDropTableCommand>(dropCommand);
            Assert.That(dropCommand.TableName, Is.EqualTo("products"));

        }

    }
}
