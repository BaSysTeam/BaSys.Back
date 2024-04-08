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
    public sealed class UpdateBuilder
    {
        private readonly UpdateModel _model;

        public UpdateBuilder()
        {
            _model = new UpdateModel();
        }

        public UpdateBuilder(UpdateModel model)
        {
            _model = model;
        }

        public UpdateBuilder(IDataModelConfiguration config)
        {
            _model = new UpdateModel(config);
        }

        public UpdateBuilder Table(string tableName)
        {
            _model.TableName = tableName;
            return this;    
        }

        public UpdateBuilder Set(string leftExpression)
        {
            _model.AddSetExpression(leftExpression, string.Empty);
            return this;
        }

        public UpdateBuilder Set(string leftExpression, string rightExpression)
        {
            _model.AddSetExpression(leftExpression, rightExpression);
            return this;
        }

        public UpdateBuilder Parameter(string parameterName, object value)
        {
            _model.AddParameter(parameterName, value);
            return this;
        }

        public UpdateBuilder Parameter(string parameterName, object value, DbType dbType)
        {
            _model.AddParameter(parameterName, value, dbType);
            return this;
        }

        public UpdateBuilder WhereAnd(string whereExpression)
        {
            _model.WhereAnd(whereExpression);

            return this;
        }

        public UpdateBuilder WhereOr(string whereExpression)
        {
            _model.WhereAnd(whereExpression);

            return this;
        }

        public IQuery Query(SqlDialectKinds dbKind)
        {
            // Validate();

            IQuery query = null;

            switch (dbKind)
            {
                case SqlDialectKinds.MsSql:
                    var msGenerator = new UpdateScriptGenerator(_model, dbKind);
                    query = msGenerator.Build();
                    break;
                case SqlDialectKinds.PgSql:
                    var pgGenerator = new UpdateScriptGenerator(_model, dbKind);
                    query = pgGenerator.Build();
                    break;
                default:
                    throw new NotImplementedException($"{GetType().Name} not implemented for DbKind {dbKind}.");

            }

            return query;

        }

        public static UpdateBuilder Make() { 

            return new UpdateBuilder();
        }

        public static UpdateBuilder Make(IDataModelConfiguration config)
        {
            return new UpdateBuilder(config);
        }

    }
}
