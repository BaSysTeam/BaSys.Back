using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.Metadata.Models
{
    [MessagePackObject(keyAsPropertyName: true)]
    public sealed class MetaObjectTableColumnRenderSettings
    {
        // Have to be equal uid of MetaObjectTableColumn.
        public Guid Uid { get; set; }
        public Guid ControlKindUid { get; set; }

        public MetaObjectTableColumnRenderSettings Clone()
        {
            var clone = new MetaObjectTableColumnRenderSettings();
            clone.Uid = Uid;
            clone.ControlKindUid = ControlKindUid;
            return clone;
        }
    }
}
