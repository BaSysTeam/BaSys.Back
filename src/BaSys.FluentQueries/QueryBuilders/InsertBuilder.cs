using BaSys.FluentQueries.Abstractions;
using BaSys.FluentQueries.Enums;
using BaSys.FluentQueries.Models;
using BaSys.FluentQueries.ScriptGenerators;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaSys.FluentQueries.QueryBuilders
{
    public sealed class InsertBuilder
    {
        private readonly InsertModel _model;

        public InsertBuilder()
        {
            _model = new InsertModel();
        }

        public InsertBuilder(InsertModel model)
        {
            _model = model;
        }

        public InsertBuilder Table(string tableName)
        {
            _model.TableName = tableName;
            return this;
        }

        public InsertBuilder Column(string columnName)
        {
            _model.AddColumn(columnName);
            return this;
        }

        public InsertBuilder FillValuesByColumnNames(bool fillValuesByColumnNames)
        {
            _model.FillValuesByColumnNames = true;
            return this;
        }

        public InsertBuilder Value(string parameterName)
        {
            _model.AddParameter(parameterName, null);
            return this;
        }

        public InsertBuilder Value(string parameterName, object value)
        {
            _model.AddParameter(parameterName, value);
            return this;
        }

        public IQuery Query(SqlDialectKinds dbKind)
        {
            // Validate();

            IQuery query = null;

            switch (dbKind)
            {
                case SqlDialectKinds.MsSql:
                    var msGenerator = new InsertScriptGenerator(_model, dbKind);
                    query = msGenerator.Build();
                    break;
                case SqlDialectKinds.PgSql:
                    var pgGenerator = new InsertScriptGenerator(_model, dbKind);
                    query = pgGenerator.Build();
                    break;
                default:
                    throw new NotImplementedException($"{GetType().Name} not implemented for DbKind {dbKind}.");

            }

            return query;

        }

        public static InsertBuilder Make()
        {
            return new InsertBuilder();
        }
    }
}
