using BaSys.FluentQueries.Enums;
using BaSys.FluentQueries.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaSys.FluentQueries.ScriptGenerators
{
    internal class PgSqlAlterTableScriptGenerator: AlterTableScriptGeneratorBase
    {
        public PgSqlAlterTableScriptGenerator(AlterTableModel model):base(SqlDialectKinds.PgSql, model)
        {
            
        }
    }
}
