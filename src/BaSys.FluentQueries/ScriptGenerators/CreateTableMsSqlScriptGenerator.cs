using BaSys.FluentQueries.Abstractions;
using BaSys.FluentQueries.Helpers;
using BaSys.FluentQueries.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace BaSys.FluentQueries.ScriptGenerators
{
    internal class CreateTableMsSqlScriptGenerator : CreateTableScriptGeneratorBase
    {

        public CreateTableMsSqlScriptGenerator(CreateTableModel model) : base(Enums.SqlDialectKinds.MsSql, model)
        {

        }

        protected override string GeneratePrimaryKey(TableColumn column)
        {
            
            string expression = $"{column.Name} {GetDataType(column.DbType, column.StringLength)} ";
            switch (column.DbType)
            {
                case DbType.Int16:
                case DbType.Int32:
                case DbType.Int64:
                    expression += "IDENTITY(1,1) PRIMARY KEY";
                    break;
                case DbType.Guid:
                   //expression += $"DEFAULT NEWID() PRIMARY KEY";
                    expression += $"PRIMARY KEY";

                    break;
                default:
                    expression += $"PRIMARY KEY";
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
