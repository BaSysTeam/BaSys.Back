using BaSys.FluentQueries.Models;
using BaSys.FluentQueries.QueryBuilders;
using BaSys.Host.DAL.Abstractions;
using Dapper;
using System.Data;

namespace BaSys.Host.DAL.DataProviders
{
    public sealed class QueriesProvider : DataProviderBase
    {

        public QueriesProvider(IDbConnection connection) : base(connection)
        {
           
        }

        public async Task<DataTable> ExecuteAsync(SelectModel model, IDbTransaction? transaction)
        {
            var builder = new SelectBuilder(model);
            _query = builder.Query(_sqlDialect);

            var dynamicCollection = await _connection.QueryAsync(_query.Text, _query.DynamicParameters, transaction);

            var dataTable = ConvertToDataTable(dynamicCollection);

            return dataTable;
        }

        private DataTable ConvertToDataTable(IEnumerable<dynamic> dynamicCollection)
        {
            var dataTable = new DataTable();

            if (!dynamicCollection.Any())
                return dataTable;

            // Using the first element to determine column names and types
            var firstElement = (IDictionary<string, object>)dynamicCollection.First();

            foreach (var key in firstElement.Keys)
            {
                dataTable.Columns.Add(key, firstElement[key]?.GetType() ?? typeof(object));
            }

            foreach (var item in dynamicCollection)
            {
                var row = dataTable.NewRow();
                var dict = (IDictionary<string, object>)item;

                foreach (var key in dict.Keys)
                {
                    row[key] = dict[key] ?? DBNull.Value;
                }

                dataTable.Rows.Add(row);
            }

            return dataTable;
        }

    }
}
