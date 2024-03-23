using BaSys.FluentQueries.Abstractions;
using BaSys.FluentQueries.Enums;
using BaSys.FluentQueries.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace BaSys.FluentQueries.QueryBuilders
{
    public sealed class CreateTableBuilder
    {
        private readonly CreateTableModel _model;
        private DbKinds _dbKind;

        public CreateTableBuilder(DbKinds dbKind)
        {
            _model = new CreateTableModel();
            _dbKind = dbKind;
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

        public CreateTableBuilder NumberColumn(string name,
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
                    query = MsSqlQuery();
                    break;
                case DbKinds.PgSql:
                    query = PgSqlQuery();
                    break;
                default:
                    throw new NotImplementedException($"{GetType().Name} not implemented for DbKind {_dbKind}.");

            }

            return query;

        }

        private string MsSqlDataType(DbType dbType, int stringLength)
        {
            var dataType = string.Empty;
            var stringLengthValue = stringLength == 0 ? "MAX" : stringLength.ToString();

            switch (dbType)
            {
                case DbType.AnsiString:
                    dataType = "VARCHAR(MAX)";
                    break;
                case DbType.AnsiStringFixedLength:
                    dataType = "CHAR";
                    break;
                case DbType.Binary:
                    dataType = $"VARBINARY({stringLengthValue})";
                    break;
                case DbType.Boolean:
                    dataType = "BIT";
                    break;
                case DbType.Byte:
                    dataType = "TINYINT";
                    break;
                case DbType.Currency:
                    dataType = "MONEY";
                    break;
                case DbType.Date:
                    dataType = "DATE";
                    break;
                case DbType.DateTime:
                    dataType = "DATETIME";
                    break;
                case DbType.DateTime2:
                    dataType = "DATETIME2";
                    break;
                case DbType.DateTimeOffset:
                    dataType = "DATETIMEOFFSET";
                    break;
                case DbType.Decimal:
                    dataType = "DECIMAL";
                    break;
                case DbType.Double:
                    dataType = "FLOAT";
                    break;
                case DbType.Guid:
                    dataType = "UNIQUEIDENTIFIER";
                    break;
                case DbType.Int16:
                    dataType = "SMALLINT";
                    break;
                case DbType.Int32:
                    dataType = "INT";
                    break;
                case DbType.Int64:
                    dataType = "BIGINT";
                    break;
                case DbType.Object:
                    dataType = "SQL_VARIANT";
                    break;
                case DbType.SByte:
                    // SQL Server does not have a direct mapping; using TINYINT as closest alternative
                    dataType = "TINYINT";
                    break;
                case DbType.Single:
                    dataType = "REAL";
                    break;
                case DbType.String:
                    dataType = $"NVARCHAR({stringLengthValue})";
                    break;
                case DbType.StringFixedLength:
                    dataType = "NCHAR";
                    break;
                case DbType.Time:
                    dataType = "TIME";
                    break;
                case DbType.UInt16:
                    // SQL Server does not have a direct mapping for unsigned types; using SMALLINT as closest alternative
                    dataType = "SMALLINT";
                    break;
                case DbType.UInt32:
                    // SQL Server does not have a direct mapping for unsigned types; using INT as closest alternative, noting the potential for overflow
                    dataType = "INT";
                    break;
                case DbType.UInt64:
                    // SQL Server does not have a direct mapping for unsigned types; using BIGINT as closest alternative, noting the potential for overflow
                    dataType = "BIGINT";
                    break;
                case DbType.VarNumeric:
                    dataType = "NUMERIC";
                    break;
                case DbType.Xml:
                    dataType = "XML";
                    break;
                default:
                    throw new NotImplementedException($"Mapping for data type {dbType} not implemented.");
            }

            return dataType;
        }


        private void MsSqlAddColumnQuery(StringBuilder sb, TableColumn column)
        {
            sb.Append(column.Name);
            sb.Append(' ');
            sb.Append(MsSqlDataType(column.DbType, column.StringLength));
            sb.Append(' ');
            if (column.PrimaryKey)
            {
                sb.Append("PRIMARY KEY");
            }
            if (column.Required)
            {
                sb.Append("NOT NULL");
            }
            else
            {
                sb.Append("NULL");
            }
        }

        private IQuery MsSqlQuery()
        {
            var query = new Query();

            var sb = new StringBuilder();
            sb.AppendLine($"CREATE TABLE {_model.TableName}(");
            foreach(var column in _model.Columns)
            {
                MsSqlAddColumnQuery (sb, column);
            }
            sb.AppendLine(")");

            query.Text = sb.ToString();
            return query;
        }

        private string PgSqlDataType(DbType dbType, int stringLength)
        {
            var dataType = string.Empty;
            // In PostgreSQL, there is no 'MAX' keyword like in SQL Server. 
            // For unlimited length text or binary columns, use 'text' or 'bytea'.
            var stringLengthValue = stringLength <= 0 ? "text" : stringLength.ToString();

            switch (dbType)
            {
                case DbType.AnsiString:
                    dataType = stringLength <= 0 ? "text" : $"varchar({stringLengthValue})";
                    break;
                case DbType.AnsiStringFixedLength:
                    dataType = $"char({stringLengthValue})";
                    break;
                case DbType.Binary:
                    dataType = "bytea";
                    break;
                case DbType.Boolean:
                    dataType = "boolean";
                    break;
                case DbType.Byte:
                    // PostgreSQL does not have a tinyint; smallint is the closest.
                    dataType = "smallint";
                    break;
                case DbType.Currency:
                    // money type is available but using numeric for precision and scale is generally recommended.
                    dataType = "money";
                    break;
                case DbType.Date:
                    dataType = "date";
                    break;
                case DbType.DateTime:
                    // timestamp without time zone is generally used; for with time zone use "timestamptz"
                    dataType = "timestamp";
                    break;
                case DbType.DateTime2:
                    // PostgreSQL does not differentiate between DateTime and DateTime2
                    dataType = "timestamp";
                    break;
                case DbType.DateTimeOffset:
                    // PostgreSQL uses "timestamptz" for timestamp with time zone.
                    dataType = "timestamptz";
                    break;
                case DbType.Decimal:
                    dataType = "numeric";
                    break;
                case DbType.Double:
                    dataType = "double precision";
                    break;
                case DbType.Guid:
                    dataType = "uuid";
                    break;
                case DbType.Int16:
                    dataType = "smallint";
                    break;
                case DbType.Int32:
                    dataType = "integer";
                    break;
                case DbType.Int64:
                    dataType = "bigint";
                    break;
                case DbType.Object:
                    // There's no direct equivalent to SQL Server's sql_variant in PostgreSQL; using JSONB for complex structures.
                    dataType = "jsonb";
                    break;
                case DbType.SByte:
                    // PostgreSQL does not have a tinyint; smallint is the closest.
                    dataType = "smallint";
                    break;
                case DbType.Single:
                    dataType = "real";
                    break;
                case DbType.String:
                    dataType = stringLength <= 0 ? "text" : $"varchar({stringLengthValue})";
                    break;
                case DbType.StringFixedLength:
                    dataType = $"char({stringLengthValue})";
                    break;
                case DbType.Time:
                    dataType = "time";
                    break;
                case DbType.UInt16:
                    // PostgreSQL does not have unsigned integers; smallint or integer might be used, with care to avoid overflow.
                    dataType = "integer";
                    break;
                case DbType.UInt32:
                    // PostgreSQL does not have unsigned integers; bigint might be used, with care to avoid overflow.
                    dataType = "bigint";
                    break;
                case DbType.UInt64:
                    // No direct unsigned large integer type in PostgreSQL; numeric can be used for large numbers, with care to check ranges.
                    dataType = "numeric";
                    break;
                case DbType.VarNumeric:
                    dataType = "numeric";
                    break;
                case DbType.Xml:
                    dataType = "xml";
                    break;
                default:
                    throw new NotImplementedException($"Mapping for data type {dbType} not implemented.");
            }

            return dataType;
        }


        private IQuery PgSqlQuery()
        {
            var query = new Query();

            var sb = new StringBuilder();
            sb.AppendLine($"CREATE TABLE {_model.TableName}(");
            foreach (var column in _model.Columns)
            {

            }
            sb.AppendLine(");");

            query.Text = sb.ToString();
            return query;
        }

        public static CreateTableBuilder Make(DbKinds dbKind)
        {
            return new CreateTableBuilder(dbKind);
        }
    }
}
