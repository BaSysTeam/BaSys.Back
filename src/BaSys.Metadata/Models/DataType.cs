using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace BaSys.Metadata.Models
{
    /// <summary>
    /// Class describes inner type in BaSys Application.
    /// </summary>
    public sealed class DataType
    {
        public Guid Uid { get; }
        public string Title { get; set; } = string.Empty;
        public bool IsPrimitive { get; set; }
        public DbType DbType { get; set; }
        public Guid? ObjectKindUid { get; set; }

        public DataType(Guid uid)
        {
            Uid = uid;
        }

        public override string ToString()
        {
            return Title;
        }

        private bool Equals(DataType other)
        {
            return Uid.Equals(other.Uid);
        }

        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj) || obj is DataType other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Uid.GetHashCode();
        }
    }
}
