using BaSys.DTO.App;
using BaSys.Metadata.Abstractions;
using BaSys.Metadata.Models;
using Humanizer;
using System.Data;
using System.Globalization;
using System.Text.Json;

namespace BaSys.App.Services
{
    public class DataObjectParser
    {
        public static DataObjectDto Parse(DataObjectDto input,
            MetaObjectStorableSettings metaObjectSettings,
            IDataTypesIndex dataTypesIndex)
        {
            var item = new DataObjectDto();
            item.Header = ParseHeader(input.Header, metaObjectSettings, dataTypesIndex);
            foreach(var table in input.DetailsTables)
            {
                var parsedTable = ParseTable(table, metaObjectSettings, dataTypesIndex);
                item.DetailsTables.Add(parsedTable);
            }

            return item;
        }

        public static DataObjectDetailsTableDto ParseTable(DataObjectDetailsTableDto inputTable, 
            MetaObjectStorableSettings metaObjectSettings, 
            IDataTypesIndex dataTypesIndex) { 

            var table = new DataObjectDetailsTableDto();
            table.Name = inputTable.Name;
            table.Uid = inputTable.Uid;
           
            var tableSettings = metaObjectSettings.DetailTables.FirstOrDefault(x=>x.Uid == inputTable.Uid);

            if (tableSettings == null)
            {
                throw new ArgumentNullException(nameof(tableSettings), $"Cannot find details table {inputTable.Name}/{inputTable.Uid}");
            }

            foreach (var row in inputTable.Rows) {

                var parsedRow = new Dictionary<string, object>();
                foreach (var kvp in row)
                {
                   

                    var fieldName = kvp.Key;
                    if (kvp.Value == null)
                        continue;
                    var jsonValue = (JsonElement)kvp.Value;

                    var fieldSettings = tableSettings.Columns.FirstOrDefault(x => x.Name.Equals(fieldName, StringComparison.OrdinalIgnoreCase));

                    if (fieldSettings == null)
                    {
                        continue;
                    }

                    var fieldDataType = dataTypesIndex.GetDataType(fieldSettings.DataTypeUid);

                    if (fieldDataType == null)
                    {
                        continue;
                    }

                    var strValue = jsonValue.ToString();
                    switch (fieldDataType.DbType)
                    {
                        case DbType.String:

                            parsedRow.Add(fieldName, strValue);
                            break;

                        case DbType.Int32:

                            int.TryParse(strValue, out int intValue);
                            parsedRow.Add(fieldName, intValue);
                            break;

                        case DbType.Int64:

                            long.TryParse(strValue, out var longValue);
                            parsedRow.Add(fieldName, longValue);
                            break;

                        case DbType.Decimal:

                            decimal.TryParse(strValue, NumberStyles.Any, CultureInfo.InvariantCulture, out var decimalValue);
                            parsedRow.Add(fieldName, decimalValue);
                            break;

                        case DbType.Guid:

                            Guid.TryParse(strValue, out var guidValue);
                            parsedRow.Add(fieldName, guidValue);
                            break;

                        case DbType.DateTime:

                            DateTime.TryParse(strValue, out var dateTimeValue);
                            parsedRow.Add(fieldName, dateTimeValue);
                            break;

                        case DbType.Byte:
                        case DbType.Boolean:

                            Boolean.TryParse(strValue, out bool boolValue);
                            parsedRow.Add(fieldName, boolValue);
                            break;
                    }

                }
                table.Rows.Add(parsedRow);
            }

            return table;
        }

        public static Dictionary<string, object> ParseHeader(Dictionary<string, object> headerJson,
            MetaObjectStorableSettings metaObjectSettings,
            IDataTypesIndex dataTypesIndex)
        {
            var headerParsed = new Dictionary<string, object>();

            foreach (var kvp in headerJson)
            {
                var fieldName = kvp.Key;
                if (kvp.Value == null)
                    continue;
                var jsonValue = (JsonElement)kvp.Value;

                var fieldSettings = metaObjectSettings.Header.Columns.FirstOrDefault(x => x.Name.Equals(fieldName, StringComparison.OrdinalIgnoreCase));

                if (fieldSettings == null)
                {
                    continue;
                }

                var fieldDataType = dataTypesIndex.GetDataType(fieldSettings.DataTypeUid);

                if (fieldDataType == null)
                {
                    continue;
                }

                var strValue = jsonValue.ToString();
                switch (fieldDataType.DbType)
                {
                    case DbType.String:

                        headerParsed.Add(fieldName, strValue);
                        break;

                    case DbType.Int32:

                        int.TryParse(strValue, out int intValue);
                        headerParsed.Add(fieldName, intValue);
                        break;

                    case DbType.Int64:

                        long.TryParse(strValue, out var longValue);
                        headerParsed.Add(fieldName, longValue);
                        break;

                    case DbType.Decimal:

                        decimal.TryParse(strValue, NumberStyles.Any, CultureInfo.InvariantCulture, out var decimalValue);
                        headerParsed.Add(fieldName, decimalValue);
                        break;

                    case DbType.Guid:

                        Guid.TryParse(strValue, out var guidValue);
                        headerParsed.Add(fieldName, guidValue);
                        break;

                    case DbType.DateTime:

                        DateTime.TryParse(strValue, out var dateTimeValue);
                        headerParsed.Add(fieldName, dateTimeValue);
                        break;

                    case DbType.Byte:
                    case DbType.Boolean:

                        Boolean.TryParse(strValue, out bool boolValue);
                        headerParsed.Add(fieldName, boolValue);
                        break;
                }

            }

            return headerParsed;
        }
    }
}
