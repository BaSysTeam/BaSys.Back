using BaSys.Common.Enums;
using System.Data;
using System.Globalization;
using System;

namespace BaSys.Common.Helpers
{
    public sealed class ValueParser
    {
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
