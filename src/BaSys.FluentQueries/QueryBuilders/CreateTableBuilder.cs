using BaSys.FluentQueries.Abstractions;
using BaSys.FluentQueries.Enums;
using BaSys.FluentQueries.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaSys.FluentQueries.QueryBuilders
{
    public sealed class CreateTableBuilder
    {
        private readonly CreateTableModel _model;
        private DbKinds _dbKind;

        public CreateTableBuilder(DbKinds dbKind)
        {
            _model = new CreateTableModel();
            _dbKind = dbKind;
        }

        public CreateTableBuilder Table(string tableName)
        {
            _model.TableName = tableName;
            return this;
        }

        public IQuery Query()
        {
            IQuery query = null;

            switch (_dbKind)
            {
                case DbKinds.MsSql:
                    query = MsSqlQuery();
                    break;
                case DbKinds.PgSql:
                    query = PgSqlQuery();
                    break;
                default:
                    throw new NotImplementedException($"{GetType().Name} not implemented for DbKind {_dbKind}.");
                    
            }

            return query;
          
        }

        private IQuery MsSqlQuery()
        {
            var query = new Query();

            var sb = new StringBuilder();
            sb.AppendLine($"CREATE TABLE {_model.TableName}(");
            sb.AppendLine(")");

            query.Text = sb.ToString();
            return query;
        }

        private IQuery PgSqlQuery()
        {
            var query = new Query();
          
            var sb = new StringBuilder();
            sb.AppendLine($"CREATE TABLE {_model.TableName}(");
            sb.AppendLine(");");

            query.Text = sb.ToString();
            return query;
        }

        public static CreateTableBuilder Make(DbKinds dbKind)
        {
            return new CreateTableBuilder(dbKind);
        }
    }
}
