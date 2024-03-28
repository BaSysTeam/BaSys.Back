using BaSys.FluentQueries.Abstractions;
using BaSys.FluentQueries.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace BaSys.FluentQueries.ScriptGenerators
{
    internal class MsSqlCreateTableScriptGenerator : CreateTableScriptGenerator
    {

        public MsSqlCreateTableScriptGenerator(CreateTableModel model) : base(model)
        {

        }

        protected override string GetDataType(DbType dbType, int stringLength)
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

    }
}
