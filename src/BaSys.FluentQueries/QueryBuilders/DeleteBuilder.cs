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
    public sealed class DeleteBuilder
    {
        private readonly DeleteModel _model;

        public DeleteBuilder()
        {
            _model = new DeleteModel();
        }

        public DeleteBuilder(DeleteModel model)
        {
            _model = model;
        }

        public DeleteBuilder Table(string tableName)
        {
            _model.TableName = tableName;
            return this;
        }

        public DeleteBuilder WhereAnd(string whereExpression)
        {
            _model.WhereAnd(whereExpression);

            return this;
        }

        public DeleteBuilder WhereOr(string whereExpression)
        {
            _model.WhereAnd(whereExpression);

            return this;
        }

        public DeleteBuilder Parameter(string parameterName, object value)
        {
            _model.AddParameter(parameterName, value);
            return this;
        }

        public DeleteBuilder Parameter(string parameterName, object value, DbType dbType)
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
                    var generator = new DeleteScriptGenerator(_model, sqlDialect);
                    query = generator.Build();
                    break;
                default:
                    throw new NotImplementedException($"{GetType().Name} not implemented for DbKind {sqlDialect}.");

            }

            return query;

        }

        public static DeleteBuilder Make() { 

            return new DeleteBuilder();
        }
    }
}
