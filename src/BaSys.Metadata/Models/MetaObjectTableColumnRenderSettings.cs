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
    }
}
