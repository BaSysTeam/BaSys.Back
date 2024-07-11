using BaSys.Metadata.Abstractions;
using BaSys.Metadata.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace BaSys.Metadata.Helpers
{
    public sealed class DataTypesIndex : IDataTypesIndex
    {
        private readonly DataType _defaultDataType = DataTypeDefaults.String;
        private readonly Dictionary<Guid, DataType> _dataTypesIndex = new Dictionary<Guid, DataType>();

        public DataTypesIndex(IEnumerable<DataType> dataTypes)
        {
            if (dataTypes != null)
            {
                _dataTypesIndex = dataTypes.ToDictionary(x => x.Uid, x => x);
            }
        }

        public bool IsDataType(Guid uid)
        {
            return _dataTypesIndex.ContainsKey(uid);
        }

        public DataType GetDataType(Guid uid)
        {
            return _dataTypesIndex[uid];
        }

        public DbType GetDbType(Guid uid)
        {
            var dataType = GetDataType(uid);

            return dataType.DbType;
        }

        public DataType GetDataTypeSafe(Guid uid)
        {
            if (_dataTypesIndex.ContainsKey(uid))
            {
                return _dataTypesIndex[uid];
            }
            else
            {
                return _defaultDataType;
            }
        }

        public DbType GetDbTypeSafe(Guid uid)
        {
            var dataType = GetDataTypeSafe(uid);

            return dataType.DbType;
        }
    }
}
