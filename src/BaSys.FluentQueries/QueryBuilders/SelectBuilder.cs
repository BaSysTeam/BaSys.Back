using BaSys.FluentQueries.Abstractions;
using BaSys.FluentQueries.Enums;
using BaSys.FluentQueries.Models;
using BaSys.FluentQueries.ScriptGenerators;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace BaSys.FluentQueries.QueryBuilders
{
    public sealed class SelectBuilder
    {
        private readonly SelectModel _model;

        public SelectBuilder()
        {
            _model = new SelectModel();
        }

        public SelectBuilder(SelectModel model)
        {
            _model = model;
        }

        public SelectBuilder From(string fromExpression)
        {
            _model.FromExpression = fromExpression;
            return this;
        }

        public SelectBuilder Select(string selectExpression)
        {
            _model.AddSelectExpression(selectExpression);
            return this;
        }

        public SelectBuilder WhereAnd(string whereExpression)
        {
            _model.WhereAnd(whereExpression);

            return this;
        }

        public SelectBuilder WhereOr(string whereExpression)
        {
            _model.WhereAnd(whereExpression);

            return this;
        }

        public SelectBuilder Parameter(string parameterName, object value)
        {
            _model.AddParameter(parameterName, value);
            return this;
        }

        public SelectBuilder Parameter(string parameterName, object value, DbType dbType)
        {
            _model.AddParameter(parameterName, value, dbType);
            return this;
        }

        public IQuery Query(SqlDialectKinds sqlDialect)
        {
            // Validate();

            IQuery query = null;

            switch (sqlDialect)
            {
                case SqlDialectKinds.MsSql:
                case SqlDialectKinds.PgSql:
                    var generator = new SelectScriptGenerator(_model, sqlDialect);
                    query = generator.Build();
                    break;
                default:
                    throw new NotImplementedException($"{GetType().Name} not implemented for DbKind {sqlDialect}.");

            }

            return query;

        }

        public static SelectBuilder Make()
        {
            return new SelectBuilder();
        }
    }
}
