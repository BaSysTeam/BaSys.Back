using System.Data;

namespace BaSys.DTO.Core
{
    public sealed class DataTableDto
    {
        private const string StringTypeName = "string";
        private const string DateTypeName = "date";
        private const string NumberTypeName = "number";
        private const string BooleanTypeName = "boolean";

        public List<DataTableColumnDto> Columns { get; set; } = new List<DataTableColumnDto>();
        public List<Dictionary<string, object>> Rows { get; set; } = new List<Dictionary<string, object>>();

        public DataTableDto()
        {

        }

        public DataTableDto(DataTable dataTable)
        {
            Convert(dataTable);
        }

        public void Convert(DataTable dataTable)
        {
            ConvertColumns(dataTable);
            ConvertRows(dataTable);
        }

        public void Clear()
        {
            Columns.Clear();
            Rows.Clear();
        }

        private void ConvertColumns(DataTable dataTable)
        {

            foreach (DataColumn column in dataTable.Columns)
            {
                var newColumn = new DataTableColumnDto()
                {
                    Name = column.ColumnName,
                    DataType = ConvertType(column.DataType),
                };
                Columns.Add(newColumn);
            }

        }

        private void ConvertRows(DataTable dataTable)
        {
            foreach (DataRow dataTableRow in dataTable.Rows)
            {
                var newRow = new Dictionary<string, object>();

                foreach (DataColumn column in dataTable.Columns)
                {
                    newRow.Add(column.ColumnName, dataTableRow[column.ColumnName]);
                }

                Rows.Add(newRow);
            }
        }

        private string ConvertType(Type type)
        {

            var result = StringTypeName;

            if (type == typeof(string)
                || type == typeof(Guid))
            {
                result = StringTypeName;
            }
            else if (type == typeof(DateTime))
            {

                result = DateTypeName;
            }
            else if (type == typeof(bool))
            {
                result = BooleanTypeName;
            }
            else if (IsNumericType(type))
            {
                result = NumberTypeName;
            }

            return result;
        }

        private bool IsNumericType(Type type)
        {
            return type == typeof(byte) ||
                   type == typeof(sbyte) ||
                   type == typeof(short) ||
                   type == typeof(ushort) ||
                   type == typeof(int) ||
                   type == typeof(uint) ||
                   type == typeof(long) ||
                   type == typeof(ulong) ||
                   type == typeof(float) ||
                   type == typeof(double) ||
                   type == typeof(decimal);
        }
    }
}
