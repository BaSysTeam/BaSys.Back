using BaSys.FluentQueries.Abstractions;
using BaSys.FluentQueries.Enums;
using BaSys.FluentQueries.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace BaSys.FluentQueries.QueryBuilders
{
    public sealed class CreateTableConstructor
    {
        private readonly CreateTableModel _model;
        private DbKinds _dbKind;

        public CreateTableConstructor(DbKinds dbKind)
        {
            _model = new CreateTableModel();
            _dbKind = dbKind;
        }

        public CreateTableConstructor(CreateTableModel model, DbKinds dbKind)
        {
            _model = model;
            _dbKind = dbKind;
        }

        public CreateTableConstructor Table(string tableName)
        {
            _model.TableName = tableName;
            return this;
        }

        public CreateTableConstructor PrimaryKey(string name, DbType dbType)
        {
            var newColumn = new TableColumn() { Name = name, DbType = dbType, PrimaryKey = true };
            _model.AddColumn(newColumn);

            return this;
        }

        public CreateTableConstructor Column(string name, DbType dbType, bool required = true, bool unique = false)
        {
            var newColumn = new TableColumn()
            {
                Name = name,
                DbType = dbType,
                Required = required,
                Unique = unique
            };

            _model.AddColumn(newColumn);

            return this;
        }

        public CreateTableConstructor StringColumn(string name,
            DbType dbType,
            int stringLength,
            bool required = true,
            bool unique = false)
        {
            var newColumn = new TableColumn()
            {
                Name = name,
                DbType = dbType,
                StringLength = stringLength,
                Required = required,
                Unique = unique
            };

            _model.AddColumn(newColumn);

            return this;
        }

        public CreateTableConstructor NumberColumn(string name,
           DbType dbType,
           int numberDigits,
           bool required = true,
           bool unique = false)
        {
            var newColumn = new TableColumn()
            {
                Name = name,
                DbType = dbType,
                NumberDigits = numberDigits,
                Required = required,
                Unique = unique
            };

            _model.AddColumn(newColumn);

            return this;
        }

        public IQuery Query()
        {
            ///TODO:
            ///Implement validation
            ///Check TableName is not empty
            ///Check only one PrimaryKey
            ///Check unique name of columns

            IQuery query = null;

            switch (_dbKind)
            {
                case DbKinds.MsSql:
                    var msSqlBuilder = new MsSqlCreateTableBuilder(_model);
                    query = msSqlBuilder.Build();
                    break;
                case DbKinds.PgSql:
                    var pgSqlBuilder = new PgSqlCreateTableBuilder(_model);
                    query = pgSqlBuilder.Build();
                    break;
                default:
                    throw new NotImplementedException($"{GetType().Name} not implemented for DbKind {_dbKind}.");

            }

            return query;

        }
  
        public static CreateTableConstructor Make(DbKinds dbKind)
        {
            return new CreateTableConstructor(dbKind);
        }
    }
}
