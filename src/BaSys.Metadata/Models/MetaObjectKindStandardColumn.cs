﻿using MemoryPack;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaSys.Metadata.Models
{
    [MemoryPackable]
    public sealed partial class MetaObjectKindStandardColumn
    {
        public Guid Uid { get; set; } = Guid.NewGuid();
        public string Title { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public Guid DataTypeUid { get; set; }
        public int StringLength { get; set; }
        public int NumberDigits { get; set; }
        public bool IsPrimaryKey { get; set; }
        public bool IsRequired { get; set; }
        public bool IsUnique { get; set; }
        public string Memo { get; set; }
    }
}
