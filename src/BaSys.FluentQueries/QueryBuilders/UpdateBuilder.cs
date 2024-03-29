using BaSys.FluentQueries.Models;
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

        public static UpdateBuilder Make() { 

            return new UpdateBuilder();
        }

    }
}
