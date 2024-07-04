using BaSys.FluentQueries.Abstractions;
using BaSys.FluentQueries.Enums;
using BaSys.FluentQueries.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaSys.FluentQueries.ScriptGenerators
{
    public abstract class InsertScriptGeneratorBase: ScriptGeneratorBase
    {
        protected readonly InsertModel _model;

        protected InsertScriptGeneratorBase(SqlDialectKinds sqlDialect, InsertModel model):base(sqlDialect)
        {
            _model = model;
        }

        public IQuery Build()
        {
            IQuery query = new Query();

            _sb.Clear();
            AppendInsertInto();
            AppendColumns();
            AppendOutput();
            AppendValues();
            AppendReturning();
            _sb.Append(";");

            query.Text = _sb.ToString();

            return query;
        }

     

        protected void AppendInsertInto()
        {
            _sb.AppendLine($"INSERT INTO {_wrapperOpen}{_model.TableName}{_wrapperClose}");
        }

        protected void AppendColumns()
        {
            _sb.Append("(");
            var n = 1;
            foreach (var column in _model.Columns)
            {
                if (n > 1)
                    _sb.Append(", ");
                _sb.Append(_wrapperOpen);
                _sb.Append(column);
                _sb.Append(_wrapperClose);
                n++;
            }
            _sb.AppendLine(")");
        }

        protected void AppendValues()
        {
            _sb.AppendLine("VALUES");

            if (_model.FillValuesByColumnNames)
            {
                AppendParameterValues();
            }
            else
            {
                AppendRowValues();
            }
        }

        protected private void AppendParameterValues()
        {

            _sb.Append("(");
            var n = 1;
            foreach (var column in _model.Columns)
            {
                if (n > 1)
                    _sb.Append(", ");

                _sb.Append('@');
                _sb.Append(column);
                n++;
            }
            _sb.Append(")");
        }

        protected void AppendRowValues()
        {

            foreach (var row in _model.Values)
            {
                var n = 1;
                _sb.Append('(');
                foreach (var value in row)
                {

                    if (n > 1)
                        _sb.Append(", ");

                    _sb.Append('@');
                    _sb.Append(value);
                    n++;
                }
                _sb.Append(')');
            }
        }

        protected virtual void AppendReturning()
        {

        }
        protected virtual void AppendOutput()
        {

        }

    }
}
