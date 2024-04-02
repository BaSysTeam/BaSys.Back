using BaSys.FluentQueries.Abstractions;
using BaSys.FluentQueries.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaSys.FluentQueries.QueryBuilders
{
    public class ColumnExistsBuilder_2 : BaseTableBuilder<ColumnExistsBuilder_2>
    {
        private string _columnName;

        public ColumnExistsBuilder_2 Column(string columnName)
        {
            _columnName = columnName;
            return this;
        }

        public override IQuery GetMsSqlQuery()
        {
            var query = new Query();
            query.Text = $"IF EXISTS(SELECT 1 FROM sys.columns WHERE Name = N'{_columnName}' AND Object_ID = Object_ID(N'{base.TableName}')) SELECT 1 AS [Exists] ELSE SELECT 0 AS [Exists]";

            return query;
        }

        public override IQuery GetPgSqlQuery()
        {
            var query = new Query();
            query.Text = $"SELECT EXISTS (SELECT 1 FROM information_schema.columns WHERE lower(table_name)='{base.TableName}' AND lower(column_name)='{_columnName}');";

            return query;
        }

        public override ColumnExistsBuilder_2 Validate()
        {
            return base.Validate();
        }
    }
}
