using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.Metadata.Models
{
    [MessagePackObject(keyAsPropertyName: true)]
    public sealed class MetaObjectRecordsSettingsColumn
    {
        public Guid DestinationColumnUid { get; set; }
        public string Expression { get; set; } = string.Empty;

        public MetaObjectRecordsSettingsColumn Clone()
        {
            var clone = new MetaObjectRecordsSettingsColumn()
            {
                DestinationColumnUid = DestinationColumnUid,
                Expression = Expression
            };

            return clone;
        }

        public override string ToString()
        {
            return $"{DestinationColumnUid} -> {Expression}";
        }
    }
}
