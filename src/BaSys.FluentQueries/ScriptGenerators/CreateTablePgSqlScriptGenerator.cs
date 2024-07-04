using BaSys.FluentQueries.Abstractions;
using BaSys.FluentQueries.Enums;
using BaSys.FluentQueries.Helpers;
using BaSys.FluentQueries.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace BaSys.FluentQueries.ScriptGenerators
{
    internal class CreateTablePgSqlScriptGenerator : CreateTableScriptGeneratorBase
    {

        public CreateTablePgSqlScriptGenerator(CreateTableModel model) : base(SqlDialectKinds.PgSql, model)
        {

        }

        protected override string GeneratePrimaryKey(TableColumn column)
        {
            string expression = $"{column.Name} ";
            switch (column.DbType)
            {
                case DbType.Int16:
                case DbType.Int32:
                    expression += "SERIAL PRIMARY KEY";
                    break;
                case DbType.Int64:
                    expression += "BIGSERIAL PRIMARY KEY";
                    break;
                case DbType.Guid:
                    //expression += $"UUID PRIMARY KEY DEFAULT uuid_generate_v4()";
                    expression += $"UUID PRIMARY KEY";

                    break;
                default:
                    expression += $"{GetDataType(column.DbType, column.StringLength)} PRIMARY KEY";
                    break;
            }

            return expression;
        }

        protected override string GetDataType(DbType dbType, int stringLength)
        {
            var converter = new DbTypeToDataTypeConverter(_sqlDialect);
            var dataType = converter.Convert(dbType, stringLength);

            return dataType;
        }
    }
}
