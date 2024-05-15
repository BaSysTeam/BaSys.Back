using BaSys.Host.DAL.Abstractions;
using BaSys.Metadata.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.Host.DAL.Helpers
{
    public sealed class MetaObjectTableChangeAnalyser
    {
        private readonly MetaObjectTable _tableBefore;
        private readonly MetaObjectTable _tableAfter;
        private readonly List<IMetaObjectTableCommand> _commands;

        public List<IMetaObjectTableCommand> Commands => _commands;

        public MetaObjectTableChangeAnalyser(MetaObjectTable tableBefore, MetaObjectTable tableAfter)
        {
            _tableBefore = tableBefore;
            _tableAfter = tableAfter;
        }

        public void Analyze()
        {

            if (_tableBefore == null)
                throw new ArgumentNullException(nameof(_tableBefore));

            if (_tableAfter == null)
                throw new ArgumentNullException(nameof(_tableAfter));

            if (_tableAfter.Uid != _tableBefore.Uid)
                throw new ArgumentException($"Table before and table after are different.");

            if (!_tableBefore.Name.Equals(_tableAfter.Name, StringComparison.OrdinalIgnoreCase))
            {
                var changeNameCommand = new MetaObjectTableChangeNameCommand()
                {
                    TableUid = _tableBefore.Uid,
                    TableName = _tableBefore.Name,
                    TableNameNew = _tableAfter.Name,
                };

                _commands.Add(changeNameCommand);
            }

            // Find droped columns.
            foreach(var column in _tableBefore.Columns)
            {
                if (_tableAfter.Columns.All(x=>x.Uid != column.Uid))
                {
                    var dropColumnCommand = new MetaObjectTableDropColumnCommand()
                    {
                        TableName = _tableAfter.Name,
                        TableUid = _tableAfter.Uid,
                        ColumnName = column.Name,
                    };

                    _commands.Add(dropColumnCommand);
                }
            }

            // Find new columns.
            foreach (var column in _tableAfter.Columns)
            {
                if (_tableAfter.Columns.All(x=>x.Uid != column.Uid))
                {
                    var addColumnCommand = new MetaObjectTableAddColumnCommand()
                    {
                        TableUid = _tableAfter.Uid,
                        TableName = _tableAfter.Name,
                        Column = column
                    };

                    _commands.Add(addColumnCommand);    
                }
            }

        }
    }
}
