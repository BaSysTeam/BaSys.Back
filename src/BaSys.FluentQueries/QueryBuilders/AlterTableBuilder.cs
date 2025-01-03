﻿using BaSys.FluentQueries.Abstractions;
using BaSys.FluentQueries.Enums;
using BaSys.FluentQueries.Models;
using BaSys.FluentQueries.ScriptGenerators;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace BaSys.FluentQueries.QueryBuilders
{
    public sealed class AlterTableBuilder
    {
        private readonly AlterTableModel _model;

        public AlterTableBuilder()
        {
            _model = new AlterTableModel();
        }

        public AlterTableBuilder(AlterTableModel model)
        {
            _model = model;
        }

        public AlterTableBuilder Table(string tableName)
        {
            _model.TableName = tableName;
            return this;
        }

        public AlterTableBuilder AddColumn(TableColumn column)
        {
            _model.NewColumns.Add(column);
            return this;
        }

        public AlterTableBuilder DropColumn(string columnName)
        {
            _model.RemovedColumns.Add(columnName);
            return this;
        }

        public AlterTableBuilder ChangeColumn(ChangeColumnModel changeModel)
        {
            _model.ChangedColumns.Add(changeModel);
            return this;
        }

        public AlterTableBuilder Rename(string oldName, string newName)
        {
            _model.RenamedColumns.Add(new RenameColumnModel(oldName, newName));
            return this;
        }

        public static AlterTableBuilder Make()
        {
            return new AlterTableBuilder();
        }

     

        public static AlterTableBuilder Make(AlterTableModel model)
        {
            return new AlterTableBuilder(model);
        }

        public IQuery Query(SqlDialectKinds dbKind)
        {
            Validate();

            IQuery query;

            switch (dbKind)
            {
                case SqlDialectKinds.MsSql:
                    var msSqlBuilder = new AlterTableMsSqlScriptGenerator(_model);
                    query = msSqlBuilder.Build();
                    break;
                case SqlDialectKinds.PgSql:
                    var pgSqlBuilder = new AlterTablePgSqlScriptGenerator(_model);
                    query = pgSqlBuilder.Build();
                    break;
                default:
                    throw new NotImplementedException($"{GetType().Name} not implemented for DbKind {dbKind}.");

            }

            return query;

        }

        private void Validate()
        {
            if (string.IsNullOrWhiteSpace(_model.TableName))
                throw new InvalidOperationException($"{GetType().Name}. Table name cannot be null or whitespace.");

        }
    }
}
