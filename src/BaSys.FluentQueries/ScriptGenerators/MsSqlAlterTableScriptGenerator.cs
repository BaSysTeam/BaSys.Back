using BaSys.FluentQueries.Enums;
using BaSys.FluentQueries.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaSys.FluentQueries.ScriptGenerators
{
    internal class MsSqlAlterTableScriptGenerator: AlterTableScriptGeneratorBase
    {
        public MsSqlAlterTableScriptGenerator(AlterTableModel model): base(SqlDialectKinds.MsSql, model)
        {
            
        }
    }
}
