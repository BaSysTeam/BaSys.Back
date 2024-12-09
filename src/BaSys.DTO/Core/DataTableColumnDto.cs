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
