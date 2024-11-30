using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.Metadata.Models
{
    [MessagePackObject(keyAsPropertyName: true)]
    public sealed class MetaObjectKindRecordsSettings
    {
        public Guid SourceCreateRecordsColumnUid { get; set; }
        public Guid StorageMetaObjectKindUid { get; set; }
        public Guid StoragePeriodColumnUid { get; set; }
        public Guid StorageKindColumnUid { get; set; }
        public Guid StorageMetaObjectColumnUid { get; set; }
        public Guid StorageObjectColumnUid { get; set; }
        public Guid StorageRowColumnUid { get; set; }
    }
}
