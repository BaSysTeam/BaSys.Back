using BaSys.FluentQueries.Abstractions;
using BaSys.FluentQueries.Enums;
using BaSys.FluentQueries.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaSys.FluentQueries.ScriptGenerators
{
    internal class InsertScriptGenerator: ScriptGeneratorBase
    {
        private readonly InsertModel _model;

        public InsertScriptGenerator(InsertModel model, SqlDialectKinds sqlDialect):base(sqlDialect)
        {
            _model = model;
        }

        public IQuery Build()
        {
            var query = GenerateScript();

            return query;
        }

        private IQuery GenerateScript()
        {

            IQuery query = new Query();

            var sb = new StringBuilder();
            sb.AppendLine($"INSERT INTO {_wrapperOpen}{_model.TableName}{_wrapperClose}");

            sb.Append("(");
            var n = 1;
            foreach (var column in _model.Columns)
            {
                if (n > 1)
                    sb.Append(", ");
                sb.Append(_wrapperOpen);
                sb.Append(column);
                sb.Append(_wrapperClose);
                n++;
            }
            sb.AppendLine(")");
            sb.AppendLine("VALUES");

            if (_model.FillValuesByColumnNames)
            {
                sb.Append("(");
                n = 1;
                foreach (var column in _model.Columns)
                {
                    if (n > 1)
                        sb.Append(", ");
                   
                    sb.Append('@');
                    sb.Append(column);
                    n++;
                }
                sb.Append(")");
            }
            else
            {
                foreach (var row in _model.Values)
                {
                    n = 1;
                    sb.Append('(');
                    foreach (var value in row)
                    {

                        if (n > 1)
                            sb.Append(", ");

                        sb.Append('@');
                        sb.Append(value);
                        n++;
                    }
                    sb.Append(")");
                }
            }

           
            sb.Append(";");

            query.Text = sb.ToString();

            return query;
        }

     
    }
}
