using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.Helpers;
using BaSys.Metadata.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.Host.DAL.TableChangeAnalyse
{
    public sealed class MetaObjectStorableChangeAnalyser
    {
        private readonly MetaObjectStorableSettings _settingsBefore;
        private readonly MetaObjectStorableSettings _settingsAfter;

        public MetaObjectStorableChangeAnalyser(MetaObjectStorableSettings settingsBefore,
            MetaObjectStorableSettings settingsAfter)
        {
            _settingsBefore = settingsBefore;
            _settingsAfter = settingsAfter;
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

            // Find droped tables.
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

            // Find new tables.
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
    }
}
