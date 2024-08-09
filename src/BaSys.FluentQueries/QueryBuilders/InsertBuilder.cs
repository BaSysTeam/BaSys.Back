using BaSys.FluentQueries.Abstractions;
using BaSys.FluentQueries.Enums;
using BaSys.FluentQueries.Models;
using BaSys.FluentQueries.ScriptGenerators;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
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

        public InsertBuilder(IDataModelConfiguration config)
        {
            _model = new InsertModel(config);
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

        public InsertBuilder PrimaryKeyName(string pkName)
        {
            _model.PrimaryKeyName = pkName;
            return this;
        }

        public InsertBuilder ReturnId(bool returnId)
        {
            _model.ReturnId = returnId;
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
            _model.Validate();

            IQuery query = null;

            InsertScriptGeneratorBase scriptGenerator = null;
            switch (dbKind)
            {
                case SqlDialectKinds.MsSql:
                    scriptGenerator = new InsertScriptGeneratorMsSql(_model);
                    break;
                case SqlDialectKinds.PgSql:
                    scriptGenerator = new InsertScriptGeneratorPgSql(_model);
                    break;
                default:
                    throw new NotImplementedException($"{GetType().Name} not implemented for DbKind {dbKind}.");
            }

            query = scriptGenerator.Build();

            return query;

        }

        public IQuery BulkCopyQuery(SqlDialectKinds dbKind)
        {
            _model.Validate();

            IQuery query = null;
            if (dbKind == SqlDialectKinds.PgSql)
            {
                var scriptGenerator = new BulkCopyPgSqlScriptGenerator(_model, dbKind);
                query = scriptGenerator.Build();
            }
            else
            {
                throw new NotImplementedException($"COPY command not implemented for DbKind {dbKind}.");
            }

            return query;
        }

        public static InsertBuilder Make()
        {
            return new InsertBuilder();
        }

        public static InsertBuilder Make(IDataModelConfiguration config)
        {
            return new InsertBuilder(config);
        }
    }
}
