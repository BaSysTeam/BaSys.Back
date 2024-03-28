using BaSys.FluentQueries.Abstractions;
using BaSys.FluentQueries.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace BaSys.FluentQueries.ScriptGenerators
{
    internal class PgSqlCreateTableScriptGenerator : CreateTableQueryScriptGenerator
    {

        public PgSqlCreateTableScriptGenerator(CreateTableModel model) : base(model)
        {

        }

        protected override string GetDataType(DbType dbType, int stringLength)
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
    }
}
