using BaSys.Common.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.Metadata.Models
{
    public sealed class PrimitiveDataTypes
    {
        private readonly Dictionary<Guid, DataType> _dataTypes;

        public PrimitiveDataTypes()
        {
            _dataTypes = DataTypeDefaults.AllTypes().ToDictionary(x=>x.Uid, x=>x);
        }

        public DataType GetDataType(Guid uid)
        {
            return _dataTypes[uid];
        }

        public DbType GetDbType(Guid uid)
        {
            var dataType = GetDataType(uid);

            return dataType.DbType;
        }
    }
}
