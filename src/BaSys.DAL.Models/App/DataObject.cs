using BaSys.Metadata.Abstractions;
using BaSys.Metadata.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.DAL.Models.App
{
    public sealed class DataObject
    {
        public Dictionary<string, object> Header { get; set; } = new Dictionary<string, object>();
        public List<DataObjectDetailTable> DetailTables { get; set; } = new List<DataObjectDetailTable>();

        public DataObject()
        {

        }

        public DataObject(MetaObjectStorableSettings settings, IDataTypesIndex dataTypeIndex)
        {
            foreach (var column in settings.Header.Columns)
            {
                var emptyValue = GetEmptyValue(column, dataTypeIndex);
                Header.Add(column.Name, emptyValue);
            }
        }

        public DataObject(IDictionary<string, object> header)
        {
            foreach (var kvp in header)
            {
                Header.Add(kvp.Key, kvp.Value);
            }
        }

        public void CopyFrom(DataObject source)
        {
            foreach (var kvp in source.Header)
            {
                Header[kvp.Key] = kvp.Value;
            }

        }

        public object GetEmptyValue(MetaObjectTableColumn column, IDataTypesIndex dataTypeIndex)
        {
            var primitiveDataTypes = new PrimitiveDataTypes();

            var dataType = dataTypeIndex.GetDataTypeSafe(column.DataTypeUid);

            switch (dataType.DbType)
            {
                case DbType.String: return "";
                case DbType.AnsiString: return "";
                case DbType.AnsiStringFixedLength: return "";
                case DbType.Binary: return new byte[0];
                case DbType.Boolean: return false;
                case DbType.Byte: return (byte)0;
                case DbType.Currency: return 0m;
                case DbType.Date: return DateTime.MinValue;
                case DbType.DateTime: return DateTime.MinValue;
                case DbType.DateTime2: return DateTime.MinValue;
                case DbType.DateTimeOffset: return DateTimeOffset.MinValue;
                case DbType.Decimal: return 0m;
                case DbType.Double: return 0.0;
                case DbType.Guid: return Guid.Empty;
                case DbType.Int16: return (short)0;
                case DbType.Int32: return 0;
                case DbType.Int64: return 0L;
                case DbType.Object: return null;
                case DbType.SByte: return (sbyte)0;
                case DbType.Single: return 0f;
                case DbType.Time: return TimeSpan.Zero;
                case DbType.UInt16: return (ushort)0;
                case DbType.UInt32: return 0u;
                case DbType.UInt64: return 0UL;
                case DbType.VarNumeric: return 0m;
                case DbType.Xml: return "";
                default: return "";
            }
        }

        public void SetValue(string key, object value)
        {

            var keyLower = key.ToLower();
            if (Header.ContainsKey(keyLower))
            {
                Header[keyLower] = value;
            }
            else
            {
                Header.Add(keyLower, value);
            }
        }

        public T? GetValue<T>(string key)
        {
            if (Header.TryGetValue(key.ToLower(), out var value))
            {
                if (value is T tmp)
                {
                    return tmp;
                }
                else
                {
                    return default(T);
                }
            }
            else
            {
                return default(T);
            }
        }
    }
}
