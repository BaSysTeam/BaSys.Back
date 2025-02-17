using BaSys.Common.Abstractions;
using BaSys.Common.Enums;
using System;

namespace BaSys.Metadata.Models;

public sealed class AttachedFileInfo<T>: SystemObjectBase
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

    public void BeforeSave()
    {
        if (Uid == Guid.Empty)
            Uid = Guid.NewGuid();
    }
}