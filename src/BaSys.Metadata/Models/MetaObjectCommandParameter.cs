using MessagePack;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.Metadata.Models
{
    [MessagePackObject(keyAsPropertyName: true)]
    public sealed class MetaObjectCommandParameter
    {
        public string Name { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public DbType DbType { get; set; } = DbType.String;

        public MetaObjectCommandParameter Clone()
        {
            var clone = new MetaObjectCommandParameter()
            {
                Name = Name,
                Value = Value,
                DbType = DbType
            };

            return clone;
        }

        public override string ToString()
        {
            return $"{Name}:{Value}";
        }
    }
}
