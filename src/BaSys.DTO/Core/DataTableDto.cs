using System.Data;

namespace BaSys.DTO.Core
{
    public sealed class DataTableDto
    {
       

        public List<DataTableColumnDto> Columns { get; set; } = new List<DataTableColumnDto>();
        public List<Dictionary<string, object?>> Rows { get; set; } = new List<Dictionary<string, object?>>();

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

        public void AddRow(Dictionary<string, object?> row)
        {
            Rows.Add(row);
        }

        private void ConvertColumns(DataTable dataTable)
        {

            foreach (DataColumn column in dataTable.Columns)
            {
                var newColumn = new DataTableColumnDto(column);
                Columns.Add(newColumn);
            }

        }

        private void ConvertRows(DataTable dataTable)
        {
            foreach (DataRow dataTableRow in dataTable.Rows)
            {
                var newRow = new Dictionary<string, object?>();

                foreach (DataColumn column in dataTable.Columns)
                {
                    newRow.Add(column.ColumnName, dataTableRow[column.ColumnName]);
                }

                Rows.Add(newRow);
            }
        }

       
    }
}
