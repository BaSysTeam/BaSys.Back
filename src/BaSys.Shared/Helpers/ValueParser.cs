using BaSys.Common.Enums;
using System.Data;
using System.Globalization;
using System;

namespace BaSys.Common.Helpers
{
    public sealed class ValueParser
    {

        public static object Parse(object input, DbType dbType)
        {
            switch (dbType)
            {
                case DbType.String:
                    return input?.ToString();

                case DbType.Int32:
                    if (input is int intValue)
                        return intValue;
                    return Parse(input?.ToString(), dbType);

                case DbType.Int64:
                    if (input is long longValue)
                        return longValue;
                    return Parse(input?.ToString(), dbType);

                case DbType.Decimal:
                    if (input is decimal decimalValue)
                        return decimalValue;
                    return Parse(input?.ToString(), dbType);

                case DbType.Guid:
                    if (input is Guid guidValue)
                        return guidValue;
                    return Parse(input?.ToString(), dbType);

                case DbType.DateTime:
                    if (input is DateTime dateTimeValue)
                        return dateTimeValue;
                    return Parse(input?.ToString(), dbType);

                case DbType.Byte:
                    if (input is byte byteValue)
                        return byteValue;
                    return Parse(input?.ToString(), dbType);

                case DbType.Boolean:
                    if (input is bool boolValue)
                        return boolValue;
                    return Parse(input?.ToString(), dbType);


                default:
                    throw new ArgumentException($"Parse for DbType {dbType} not implemented.");
            }
        }

        public static object Parse(string input, DbType dbType)
        {
            switch (dbType)
            {
                case DbType.String:

                    return input;

                case DbType.Int32:

                    if (int.TryParse(input, out int intValue))
                    {
                        return intValue;
                    }
                    else
                    {
                        return 0;
                    }


                case DbType.Int64:

                    if (long.TryParse(input, out var longValue))
                    {
                        return longValue;
                    }
                    else
                    {
                        return 0;
                    }

                case DbType.Decimal:

                    if (decimal.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out var decimalValue))
                    {
                        return decimalValue;
                    }
                    else
                    {
                        return 0M;
                    }


                case DbType.Guid:

                    if (Guid.TryParse(input, out var guidValue))
                    {
                        return guidValue;
                    }
                    else
                    {
                        return Guid.Empty;
                    }


                case DbType.DateTime:

                    if (DateTime.TryParse(input, out var dateTimeValue))
                    {
                        return dateTimeValue;
                    }
                    else
                    {
                        return DateTime.MinValue;
                    }


                case DbType.Byte:
                case DbType.Boolean:

                    if (Boolean.TryParse(input, out bool boolValue))
                    {
                        return boolValue;
                    }
                    else
                    {
                        return false;
                    }


                default:
                    throw new ArgumentException($"Parse for DbType {dbType} not implemented.");
            }
        }
    }
}
