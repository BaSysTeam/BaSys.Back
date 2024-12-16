using BaSys.Common.Helpers;
using BaSys.Metadata.Abstractions;
using BaSys.Metadata.Helpers;
using BaSys.Metadata.Models;
using System.Data;

namespace BaSys.DAL.Models.App
{
    public sealed class DataObject
    {

        private readonly MetaObjectStorableSettings _settings;

        public Dictionary<string, object?> Header { get; set; } = new Dictionary<string, object?>();
        public List<DataObjectDetailsTable> DetailTables { get; set; } = new List<DataObjectDetailsTable>();


        public DataObject(MetaObjectStorableSettings settings, IDataTypesIndex dataTypeIndex)
        {
            _settings = settings;
            foreach (var column in settings.Header.Columns)
            {

                var dataType = dataTypeIndex.GetDataTypeSafe(column.DataTypeUid);

                if (!string.IsNullOrEmpty(column.DefaultValue))
                {
                    // Get default value;
                    object defaultValue = null;
                    if (column.DefaultValue.Equals("now", StringComparison.OrdinalIgnoreCase) && dataType.Uid == DataTypeDefaults.DateTime.Uid)
                    {
                        defaultValue = DateTime.Now;
                    }
                    else
                    {
                        defaultValue = ValueParser.Parse(column.DefaultValue, dataType.DbType);
                    }

                    Header.Add(column.Name, defaultValue);
                }
                else
                {
                    var emptyValue = GetEmptyValue(column, dataType.DbType);
                    Header.Add(column.Name, emptyValue);
                }

            }
        }

        public DataObject(MetaObjectStorableSettings settings, IDictionary<string, object> header)
        {
            _settings = settings;
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

            foreach (var tableSource in source.DetailTables)
            {
                DetailTables.Add(tableSource);
            }

        }

        public object GetEmptyValue(MetaObjectTableColumn column, DbType dbType)
        {

            switch (dbType)
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

        public void SetPrimaryKey(string value)
        {
            var dataTypeIndex = new DataTypesIndex(DataTypeDefaults.GetPrimaryKeyTypes());
            var dbType = dataTypeIndex.GetDbType(_settings.Header.PrimaryKey.DataTypeUid);
            var parsedValue = ValueParser.Parse(value, dbType);

            SetValue(_settings.Header.PrimaryKey.Name, parsedValue);
        }

        public object? GetPrimaryKey()
        {
            return GetValue(_settings.Header.PrimaryKey.Name);
        }

        public void SetValue(string key, object? value)
        {
            var keyLower = key.ToLower();

            if (value is DateTime dateTimeValue)
            {
                value = DateTime.SpecifyKind(dateTimeValue, DateTimeKind.Unspecified);
            }
            SetHeaderValue(keyLower, value);
           
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

        public object? GetValue(string key, object? defaultValue = null)
        {
            if (Header.TryGetValue(key.ToLower(), out var value))
            {
                return value;
            }
            else
            {
                return defaultValue;
            }
        }

        private void SetHeaderValue(string keyLower, object? value)
        {
            if (Header.ContainsKey(keyLower))
            {
                Header[keyLower] = value;
            }
            else
            {
                Header.Add(keyLower, value);
            }
        }

    }
}
