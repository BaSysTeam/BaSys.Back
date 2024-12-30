using BaSys.FluentQueries.Models;
using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.Helpers;
using BaSys.Metadata.Abstractions;
using BaSys.Metadata.Models;
using System.ComponentModel.DataAnnotations;

namespace BaSys.Host.DAL.TableChangeAnalyse
{
    public sealed class MetaObjectTableChangeAnalyser
    {
        private readonly MetaObjectTable _tableBefore;
        private readonly MetaObjectTable _tableAfter;
        private readonly List<IMetaObjectChangeCommand> _commands = new List<IMetaObjectChangeCommand>();

        public List<IMetaObjectChangeCommand> Commands => _commands;
        public bool NeedAlterTable { get; private set; }

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
            foreach (var column in _tableBefore.Columns)
            {
                if (_tableAfter.Columns.All(x => x.Uid != column.Uid))
                {
                    var dropColumnCommand = new DropColumnCommand()
                    {
                        TableName = _tableAfter.Name,
                        TableUid = _tableAfter.Uid,
                        ColumnName = column.Name,
                    };

                    _commands.Add(dropColumnCommand);

                    NeedAlterTable = true;
                }
            }

            // Find new columns.
            foreach (var column in _tableAfter.Columns)
            {
                var columnBefore = _tableBefore.GetColumn(column.Uid);

                if (columnBefore == null)
                {
                    // Add new column.
                    var addColumnCommand = new AddColumnCommand()
                    {
                        TableUid = _tableAfter.Uid,
                        TableName = _tableAfter.Name,
                        Column = column
                    };

                    _commands.Add(addColumnCommand);
                    NeedAlterTable = true;
                }
                else 
                {
                    if (!column.Name.Equals(columnBefore.Name, StringComparison.CurrentCultureIgnoreCase))
                    {
                        // Rename column.
                        var renameColumnCommand = new RenameColumnCommand()
                        {
                            TableUid = _tableAfter.Uid,
                            TableName = _tableAfter.Name,
                            ColumnName = columnBefore.Name,
                            ColumnNameNew = column.Name
                        };

                        _commands.Add(renameColumnCommand);
                        NeedAlterTable = true;
                    }

                    if (!column.DataSettings.SettingsEquals(columnBefore.DataSettings))
                    {
                        // Data type changed.
                        var changeColumnCommand = new ChangeColumnCommand()
                        {
                            TableUid = _tableAfter.Uid,
                            TableName = _tableAfter.Name,
                            Column = column,
                            RequiredChanged = column.DataSettings.Required != columnBefore.DataSettings.Required,
                            UniqueChanged = column.DataSettings.Unique != columnBefore.DataSettings.Unique,
                            DataTypeChanged = !column.DataSettings.DataTypeEquals(columnBefore.DataSettings)
                        };
                       
                        _commands.Add(changeColumnCommand);
                        NeedAlterTable = true;

                    }
                   
                }

            }

        }

        public AlterTableModel ToAlterModel(IDataTypesIndex dataTypesIndex)
        {
            var model = new AlterTableModel();

            model.TableName = _tableAfter.Name;

            foreach (var command in _commands)
            {
                if (command is AddColumnCommand addColumnCommand)
                {
                    var tableColumn = CreateTableColumn(dataTypesIndex, addColumnCommand);
                    model.NewColumns.Add(tableColumn);
                }
                else if (command is DropColumnCommand dropColumnCommand)
                {
                    model.RemovedColumns.Add(dropColumnCommand.ColumnName);
                }
                else if (command is RenameColumnCommand renameColumnCommand)
                {
                    model.RenamedColumns.Add(new RenameColumnModel()
                    {
                        OldName = renameColumnCommand.ColumnName,
                        NewName = renameColumnCommand.ColumnNameNew
                    });
                }
                else if (command is ChangeColumnCommand changeColumnCommand)
                {

                    var tableColumn = CreateTableColumn(dataTypesIndex, changeColumnCommand);
                    var changeModel = new ChangeColumnModel();
                    changeModel.Column = tableColumn;
                    changeModel.RequiredChanged = changeColumnCommand.RequiredChanged;
                    changeModel.UniqueChanged = changeColumnCommand.UniqueChanged;
                    changeModel.DataTypeChanged = changeColumnCommand.DataTypeChanged;

                    model.ChangedColumns.Add(changeModel);
                }
            }

            return model;
        }

        private TableColumn CreateTableColumn(IDataTypesIndex dataTypesIndex, IMetaObjectChangeColumnCommand command)
        {
            var dataType = dataTypesIndex.GetDataTypeSafe(command.Column.DataSettings.DataTypeUid);
            var tableColumn = new TableColumn()
            {
                Name = command.Column.Name,
                DbType = dataType.DbType,
                Required = command.Column.DataSettings.Required,
                Unique = command.Column.DataSettings.Unique,
                StringLength = command.Column.DataSettings.StringLength,
                NumberDigits = command.Column.DataSettings.NumberDigits,
            };

            return tableColumn;
        }

       
    }
}
