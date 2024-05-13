using BaSys.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.Metadata.Models
{
    public sealed class AttachedFileInfo<T>
    {
        public Guid Uid { get; set; }

        public Guid MetaObjectKindUid {get; set;}
        public Guid MetaObjectUid { get; set; }
        public T ObjectUid { get; set; }

        public string FileName { get; set; }
        public string MimeType { get; set; }
        public bool IsImage { get; set; }
        public bool IsMainImage { get; set; }
        public FileStorageKinds StorageKind { get; set; }

        public string UserId { get; set; }
        public string UserName { get; set; }
        public DateTime UploadDate { get; set; }

    }
}
