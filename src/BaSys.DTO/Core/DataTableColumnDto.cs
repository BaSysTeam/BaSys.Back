using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.DTO.Core
{
    public sealed class DataTableColumnDto
    {
        private const string StringTypeName = "string";
        private const string DateTypeName = "date";
        private const string NumberTypeName = "number";
        private const string BooleanTypeName = "boolean";

        public string Uid { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string DataType { get; set; } = string.Empty;
        public bool IsReference { get; set; }
        public string Width { get; set; } = string.Empty;

        public DataTableColumnDto()
        {
            
        }

        public DataTableColumnDto(DataColumn column)
        {
            Name = column.ColumnName;
            DataType = ConvertType(column.DataType);
        }

        public override string ToString()
        {
            return $"{Name}/{DataType}";
        }

        public static string ConvertType(DbType dbType)
        {

            var result = StringTypeName;

            switch (dbType)
            {

                case DbType.Byte:
                case DbType.Boolean:
                    result = BooleanTypeName;
                    break;
                case DbType.Date:
                case DbType.DateTime:
                case DbType.DateTime2:
                case DbType.DateTimeOffset:
                    result = DateTypeName;
                    break;
                case DbType.Decimal:
                case DbType.Double:
                case DbType.Int16:
                case DbType.Int32:
                case DbType.Int64:
                case DbType.UInt16:
                case DbType.UInt32:
                case DbType.UInt64:
                case DbType.VarNumeric:
                    result = NumberTypeName;
                    break;
                case DbType.Object:
                case DbType.Currency:
                case DbType.SByte:
                case DbType.Single:
                case DbType.String:
                case DbType.Time:
                case DbType.AnsiStringFixedLength:
                case DbType.StringFixedLength:
                case DbType.Xml:
                case DbType.AnsiString:
                case DbType.Binary:
                case DbType.Guid:
                    result = StringTypeName;
                    break;

            }

            return result;
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
