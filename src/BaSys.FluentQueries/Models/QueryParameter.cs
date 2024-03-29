using BaSys.FluentQueries.Abstractions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace BaSys.FluentQueries.Models
{
    public class QueryParameter : IQueryParameter
    {
        public string Name { get; set; }
        public object Value { get; set; }
        public DbType DbType { get; set; }

        public QueryParameter()
        {

        }

        public QueryParameter(string name, object value)
        {
            Name = name;
            Value = value;
            DbType = DbTypeByValue(value);
        }

        public QueryParameter(string name, object value, DbType dbType)
        {
            Name = name;
            Value = value;
            DbType = dbType;
        }
        public static DbType DbTypeByValue(object value)
        {
            var dbType = DbType.Object;

            if (value == null)
            {
                return dbType;
            }

            var valueType = value.GetType();

            dbType = DbTypeByValueType(valueType);

            return dbType;
        }

        public static DbType DbTypeByValueType(Type valueType)
        {

            var dbType = DbType.Object;


            if (valueType == typeof(int))
            {
                dbType = DbType.Int32;
            }
            else if (valueType == typeof(long))
            {
                dbType = DbType.Int64;
            }
            else if (valueType == typeof(byte))
            {
                dbType = DbType.Byte;
            }
            else if (valueType == typeof(Int16))
            {
                dbType = DbType.Int16;
            }
            else if (valueType == typeof(decimal))
            {
                dbType = DbType.Decimal;
            }
            else if (valueType == typeof(double))
            {
                dbType = DbType.Double;
            }
            else if (valueType == typeof(bool))
            {
                dbType = DbType.Boolean;
            }
            else if (valueType == typeof(DateTime))
            {
                dbType = DbType.DateTime;
            }
            else if (valueType == typeof(Guid))
            {
                dbType = DbType.Guid;
            }
            else if (valueType == typeof(string))
            {
                dbType = DbType.String;
            }

            return dbType;
        }
    }
}
