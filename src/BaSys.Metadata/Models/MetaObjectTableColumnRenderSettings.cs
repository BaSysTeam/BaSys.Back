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
     
        public string ControlKindUid { get; set; }
        public bool Readonly { get; set; }
        public bool Hidden { get; set; }

        public MetaObjectTableColumnRenderSettings Clone()
        {
            var clone = new MetaObjectTableColumnRenderSettings();
            clone.ControlKindUid = ControlKindUid;
            clone.Readonly = Readonly;
            clone.Hidden = Hidden;
            return clone;
        }
    }
}
