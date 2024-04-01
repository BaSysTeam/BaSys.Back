using BaSys.FluentQueries.Abstractions;
using BaSys.FluentQueries.Enums;
using BaSys.FluentQueries.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace BaSys.FluentQueries.ScriptGenerators
{
    internal class UpdateScriptGenerator: ScriptGeneratorBase
    {
        private readonly UpdateModel _model;

        public UpdateScriptGenerator(UpdateModel model, SqlDialectKinds sqlDialect):base(sqlDialect)
        {
            _model = model;
        }

        public IQuery Build()
        {
            var query = new Query();

            return query;
        }

    }
}
