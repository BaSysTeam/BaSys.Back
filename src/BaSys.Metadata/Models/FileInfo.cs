using System;
using BaSys.Common.Enums;

namespace BaSys.Metadata.Models;

public class FileInfo
{
    public Guid Uid { get; set; }
    public Guid MetaObjectKindUid {get; set;}
    public Guid MetaObjectUid { get; set; }
    public string FileName { get; set; }
    public string MimeType { get; set; }
    public bool IsImage { get; set; }
    public bool IsMainImage { get; set; }
}