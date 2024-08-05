using BaSys.DAL.Models.App;
using BaSys.FluentQueries.Enums;
using BaSys.Metadata.Abstractions;
using BaSys.Metadata.Models;
using System.Data;
using Npgsql;
using System.Text;
using BaSys.FluentQueries.QueryBuilders;

namespace BaSys.Host.DAL.Helpers
{
    public sealed class BulkInsertHelper
    {
        private readonly IDbConnection _connection;
        private readonly SqlDialectKinds _sqlDialect;
        private readonly MetaObjectTable _metaObjectTable;
        private readonly string _tableName;
        private readonly IDataTypesIndex _dataTypesIndex;

        public BulkInsertHelper(IDbConnection connection,
            SqlDialectKinds sqlDialect,
            MetaObjectTable tableSettings,
            string tableName,
            IDataTypesIndex dataTypesIndex)
        {
            _connection = connection;
            _sqlDialect = sqlDialect;
            _metaObjectTable = tableSettings;
            _tableName = tableName;
            _dataTypesIndex = dataTypesIndex;
        }

        public void Execute(DataObjectDetailsTable table)
        {
            var dataTable = PrepareDataTable(_metaObjectTable, table, _dataTypesIndex);
            Execute(dataTable);
        }

        public void Execute(DataTable dataTable)
        {
            switch (_sqlDialect)
            {
                case SqlDialectKinds.MsSql:
                    ExecuteMsSql(dataTable);
                    break;
                case SqlDialectKinds.PgSql:
                    ExecutePgSql(dataTable);
                    break;
                default:
                    throw new NotImplementedException($"Not implemented for SQL dialect: {_sqlDialect}");
            }
        }

        public static Type ConvertDbTypeToType(DbType dbType)
        {
            switch (dbType)
            {
                case DbType.AnsiString:
                case DbType.String:
                case DbType.StringFixedLength:
                case DbType.AnsiStringFixedLength:
                    return typeof(string);
                case DbType.Binary:
                    return typeof(byte[]);
                case DbType.Byte:
                    return typeof(byte);
                case DbType.Boolean:
                    return typeof(bool);
                case DbType.Currency:
                case DbType.Decimal:
                    return typeof(decimal);
                case DbType.Date:
                case DbType.DateTime:
                case DbType.DateTime2:
                case DbType.DateTimeOffset:
                    return typeof(DateTime);
                case DbType.Double:
                    return typeof(double);
                case DbType.Guid:
                    return typeof(Guid);
                case DbType.Int16:
                    return typeof(short);
                case DbType.Int32:
                    return typeof(int);
                case DbType.Int64:
                    return typeof(long);
                case DbType.SByte:
                    return typeof(sbyte);
                case DbType.Single:
                    return typeof(float);
                case DbType.Time:
                    return typeof(TimeSpan);
                case DbType.UInt16:
                    return typeof(ushort);
                case DbType.UInt32:
                    return typeof(uint);
                case DbType.UInt64:
                    return typeof(ulong);
                case DbType.VarNumeric:
                    return typeof(decimal);
                case DbType.Xml:
                    return typeof(string); // XML can be represented as string
                case DbType.Object:
                    return typeof(object);
                default:
                    throw new ArgumentException($"Unsupported DbType: {dbType}");
            }
        }

        private void ExecuteMsSql(DataTable dataTable)
        {
            throw new NotImplementedException("Bulk insert not implemented for MS SQL");
        }
        private void ExecutePgSql(DataTable dataTable)
        {
            var npgConnection = _connection as NpgsqlConnection;
            if (npgConnection == null)
            {
                return;
            }
         
            var copyCommand = BuildCopyCommand(dataTable);
            using (var writer = npgConnection.BeginBinaryImport(copyCommand))
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    writer.StartRow();
                    foreach (DataColumn column in dataTable.Columns)
                    {
                        writer.Write(row[column.ColumnName]);
                    }
                }
                writer.Complete();
            }
        }

        private string BuildCopyCommand(DataTable dataTable)
        {
            var builder = InsertBuilder.Make().Table(_tableName);
            foreach(DataColumn column in dataTable.Columns)
            {
                builder.Column(column.ColumnName);
            }

            var query = builder.BulkCopyQuery(SqlDialectKinds.PgSql);

            return query.Text;
        }

        private DataTable PrepareDataTable(MetaObjectTable tableSettings,
            DataObjectDetailsTable table,
            IDataTypesIndex dataTypesIndex)
        {
            DataTable dataTable = new DataTable();

            foreach (var column in tableSettings.Columns)
            {
                var dataType = dataTypesIndex.GetDataTypeSafe(column.DataTypeUid);
                dataTable.Columns.Add(column.Name, ConvertDbTypeToType(dataType.DbType));

            }

            foreach (var row in table.Rows)
            {
                var dataTableRow = dataTable.NewRow();
                foreach (DataColumn dataTableColumn in dataTable.Columns)
                {
                    if (row.Fields.ContainsKey(dataTableColumn.ColumnName))
                    {
                        dataTableRow[dataTableColumn.ColumnName] = row.Fields[dataTableColumn.ColumnName];
                    }
                }

                dataTable.Rows.Add(dataTableRow);
            }

            return dataTable;
        }
    }
}
