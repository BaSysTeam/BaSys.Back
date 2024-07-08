using BaSys.Metadata.Models;
using Humanizer;
using System.Data;
using System.Globalization;
using System.Text.Json;

namespace BaSys.App.Services
{
    public class DataObjectParser
    {
        public static Dictionary<string, object> ParseHeader(Dictionary<string, object> headerJson, 
            MetaObjectStorableSettings metaObjectSettings, 
            PrimitiveDataTypes primitiveDataTypes)
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

                var fieldDataType = primitiveDataTypes.GetDataType(fieldSettings.DataTypeUid);

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
