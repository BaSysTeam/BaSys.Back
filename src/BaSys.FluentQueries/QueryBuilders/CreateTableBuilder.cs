using BaSys.FluentQueries.Abstractions;
using BaSys.FluentQueries.Enums;
using BaSys.FluentQueries.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace BaSys.FluentQueries.QueryBuilders
{
    public sealed class CreateTableBuilder
    {
        private readonly CreateTableModel _model;

        public CreateTableBuilder()
        {
            _model = new CreateTableModel();
        }

        public CreateTableBuilder(CreateTableModel model)
        {
            _model = model;
        }

        public CreateTableBuilder Table(string tableName)
        {
            _model.TableName = tableName;
            return this;
        }

        public CreateTableBuilder PrimaryKey(string name, DbType dbType)
        {
            var newColumn = new TableColumn() { Name = name, DbType = dbType, PrimaryKey = true };
            _model.AddColumn(newColumn);

            return this;
        }

        public CreateTableBuilder Column(string name, DbType dbType, bool required = true, bool unique = false)
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

        public CreateTableBuilder StringColumn(string name,
            int stringLength,
            bool required = true,
            bool unique = false)
        {
            var newColumn = new TableColumn()
            {
                Name = name,
                DbType = DbType.String,
                StringLength = stringLength,
                Required = required,
                Unique = unique
            };

            _model.AddColumn(newColumn);

            return this;
        }

        public CreateTableBuilder DecimalColumn(string name,
           int numberDigits,
           bool required = true,
           bool unique = false)
        {
            var newColumn = new TableColumn()
            {
                Name = name,
                DbType = DbType.Decimal,
                NumberDigits = numberDigits,
                Required = required,
                Unique = unique
            };

            _model.AddColumn(newColumn);

            return this;
        }

        public IQuery Query(SqlDialectKinds dbKind)
        {
            Validate();

            IQuery query = null;

            switch (dbKind)
            {
                case SqlDialectKinds.MsSql:
                    var msSqlBuilder = new MsSqlCreateTableQueryBuilder(_model);
                    query = msSqlBuilder.Build();
                    break;
                case SqlDialectKinds.PgSql:
                    var pgSqlBuilder = new PgSqlCreateTableQueryBuilder(_model);
                    query = pgSqlBuilder.Build();
                    break;
                default:
                    throw new NotImplementedException($"{GetType().Name} not implemented for DbKind {dbKind}.");

            }

            return query;

        }
  
        public static CreateTableBuilder Make()
        {
            return new CreateTableBuilder();
        }

        private void Validate()
        {
            if (string.IsNullOrWhiteSpace(_model.TableName))
                throw new InvalidOperationException($"{GetType().Name}. Table name cannot be null or whitespace.");

            if (_model.Columns.Count(c => c.PrimaryKey) > 1)
                throw new InvalidOperationException($"{GetType().Name} . Table name cannot be null or whitespace.ey.");

            if (_model.Columns.GroupBy(c => c.Name).Any(g => g.Count() > 1))
                throw new InvalidOperationException($"{GetType().Name} . Table name cannot be null or whitespace.e.");
        }
    }
}
