using BaSys.FluentQueries.Enums;
using BaSys.FluentQueries.Helpers;
using BaSys.FluentQueries.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace BaSys.FluentQueries.ScriptGenerators
{
    internal class AlterTableMsSqlScriptGenerator: AlterTableScriptGeneratorBase
    {
        public AlterTableMsSqlScriptGenerator(AlterTableModel model): base(SqlDialectKinds.MsSql, model)
        {
            
        }

        protected override string GetDataType(DbType dbType, int stringLength)
        {
            var converter = new DbTypeToDataTypeConverter(_sqlDialect);
            var dataType = converter.Convert(dbType, stringLength);
        
            return dataType;
        }
    }
}
