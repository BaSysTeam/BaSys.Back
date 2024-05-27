using BaSys.Metadata.Models;
using Humanizer;
using System.Data;
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

                switch (fieldDataType.DbType)
                {
                    case DbType.String:

                        headerParsed.Add(fieldName, jsonValue.GetString() ?? string.Empty);
                        break;

                    case DbType.Int32:

                        jsonValue.TryGetInt32(out var intValue);
                        headerParsed.Add(fieldName, intValue);
                        break;

                    case DbType.Int64:

                        jsonValue.TryGetInt64(out var longValue);
                        headerParsed.Add(fieldName, longValue);
                        break;

                    case DbType.Decimal:

                        jsonValue.TryGetDecimal(out var decimalValue);
                        headerParsed.Add(fieldName, decimalValue);
                        break;

                    case DbType.Guid:

                        jsonValue.TryGetGuid(out var guidValue);
                        headerParsed.Add(fieldName, guidValue);
                        break;

                    case DbType.DateTime:

                        jsonValue.TryGetDateTime(out var dateTimeValue);
                        headerParsed.Add(fieldName, dateTimeValue);
                        break;

                    case DbType.Boolean:

                        jsonValue.TryGetByte(out var byteValue);
                        headerParsed.Add(fieldName, byteValue == 0 ? false: true);
                        break;
                }

            }

            return headerParsed;    
        }
    }
}
