using BaSys.Host.DAL.Abstractions;
using BaSys.Metadata.Abstractions;
using BaSys.Metadata.Models;

namespace BaSys.Host.DAL.TableChangeAnalyse
{
    public sealed class MetaObjectStorableChangeAnalyser
    {
        private readonly MetaObjectStorableSettings _settingsBefore;
        private readonly MetaObjectStorableSettings _settingsAfter;
        private readonly IDataTypesIndex _dataTypeIndex;

        public MetaObjectStorableChangeAnalyser(MetaObjectStorableSettings settingsBefore,
            MetaObjectStorableSettings settingsAfter, 
            IDataTypesIndex dataTypeIndex)
        {
            _settingsBefore = settingsBefore;
            _settingsAfter = settingsAfter;
            _dataTypeIndex = dataTypeIndex;
        }

        private readonly List<IMetaObjectChangeCommand> _commands = new List<IMetaObjectChangeCommand>();

        public List<IMetaObjectChangeCommand> Commands => _commands;

        public void Analyze()
        {
            if (_settingsBefore == null)
                throw new ArgumentNullException(nameof(_settingsBefore));

            if (_settingsAfter == null)
                throw new ArgumentNullException(nameof(_settingsAfter));

            if (_settingsBefore.Uid != _settingsAfter.Uid)
                throw new ArgumentException($"MetaObject before and after are different.");

            CheckRemovedTables();
            CheckCreatedTables();
            CheckChangedTables();

        }

        private void CheckRemovedTables()
        {
            foreach (var table in _settingsBefore.DetailTables)
            {
                if (_settingsAfter.DetailTables.All(x => x.Uid != table.Uid))
                {
                    var dropCommand = new MetaObjectDropTableCommand()
                    {
                        TableName = table.Name,
                        TableUid = table.Uid,
                    };

                    _commands.Add(dropCommand);

                }
            }
        }

        private void CheckCreatedTables()
        {
            foreach (var table in _settingsAfter.DetailTables)
            {
                if (_settingsBefore.DetailTables.All(x => x.Uid != table.Uid))
                {
                    var createTableCommand = new MetaObjectCreateTableCommand()
                    {
                        TableUid = table.Uid,
                        TableName = table.Name,
                    };

                    _commands.Add(createTableCommand);
                }
            }
        }

        private void CheckChangedTables()
        {
            foreach (var table in _settingsAfter.DetailTables)
            {
                 var tableBefore = _settingsBefore.DetailTables.FirstOrDefault(x=> x.Uid == table.Uid);

                if (tableBefore == null)
                {
                    continue;
                }

                var tableChangeAnalyser = new MetaObjectTableChangeAnalyser(tableBefore, table);
                tableChangeAnalyser.Analyze();

                if (tableChangeAnalyser.NeedAlterTable)
                {
                    var newCommand = new MetaObjectAlterTableCommand()
                    {
                        TableUid = table.Uid,
                        TableName = table.Name,
                        AlterTableModel = tableChangeAnalyser.ToAlterModel(_dataTypeIndex)
                    };

                    _commands.Add(newCommand);
                }

            }
        }
    }
}
