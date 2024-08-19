using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.Metadata.Models
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class MetaObjectTableColumn: IEquatable<MetaObjectTableColumn>
    {
        public Guid Uid { get; set; } = Guid.NewGuid();
        public string Title { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public Guid DataTypeUid { get; set; }
        public int StringLength { get; set; }
        public int NumberDigits { get; set; }
        public bool PrimaryKey { get; set; }
        public bool Required { get; set; }
        public bool Unique { get; set; }
        public MetaObjectTableColumnRenderSettings RenderSettings { get; set; } = new MetaObjectTableColumnRenderSettings();
        public bool IsStandard { get; set; }

        public bool Equals(MetaObjectTableColumn other)
        {
            if (other == null) return false;

            return Uid == other.Uid &&
                   Title == other.Title &&
                   Name == other.Name &&
                   DataTypeUid == other.DataTypeUid &&
                   StringLength == other.StringLength &&
                   NumberDigits == other.NumberDigits &&
                   PrimaryKey == other.PrimaryKey &&
                   Required == other.Required &&
                   Unique == other.Unique &&
                   IsStandard == other.IsStandard;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as MetaObjectTableColumn);
        }

        public override int GetHashCode()
        {
            
            int hash1 = HashCode.Combine(Uid, Title, Name, DataTypeUid, StringLength, NumberDigits, PrimaryKey);
            int hash2 = HashCode.Combine(Required, Unique, IsStandard);

            return HashCode.Combine(hash1, hash2);
        }

        public MetaObjectTableColumn Clone()
        {
            var clone = new MetaObjectTableColumn();
            clone.Uid = Uid;
            clone.Title = Title;
            clone.Name = Name;
            clone.DataTypeUid = DataTypeUid;
            clone.StringLength = StringLength;
            clone.NumberDigits = NumberDigits;
            clone.PrimaryKey = PrimaryKey;
            clone.Unique = Unique;
            clone.IsStandard = IsStandard;

            clone.RenderSettings = RenderSettings.Clone();
            return clone;
        }
    }
}
