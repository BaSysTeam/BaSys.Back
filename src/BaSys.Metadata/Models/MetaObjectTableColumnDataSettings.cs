using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.Metadata.Models
{
    [MessagePackObject(keyAsPropertyName: true)]
    public sealed class MetaObjectTableColumnDataSettings
    {
        public Guid DataTypeUid { get; set; }
        public int StringLength { get; set; }
        public int NumberDigits { get; set; }
        public bool PrimaryKey { get; set; }
        public bool Required { get; set; }
        public bool Unique { get; set; }
        public string DefaultValue { get; set; } = String.Empty;

        public MetaObjectTableColumnDataSettings Clone()
        {
            var clone = new MetaObjectTableColumnDataSettings();
            clone.DataTypeUid = DataTypeUid;
            clone.StringLength = StringLength;
            clone.NumberDigits = NumberDigits;
            clone.PrimaryKey = PrimaryKey;
            clone.Required = Required;
            clone.Unique = Unique;
            clone.DefaultValue = DefaultValue;

            return clone;
        }

        public override bool Equals(object obj)
        {
            return obj is MetaObjectTableColumnDataSettings settings &&
                   DataTypeUid.Equals(settings.DataTypeUid) &&
                   StringLength == settings.StringLength &&
                   NumberDigits == settings.NumberDigits;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(DataTypeUid, StringLength, NumberDigits);
        }
    }
}
