﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.Metadata.DTOs
{
    public sealed class MetaObjectStorableSettingsDto
    {
        public string Uid { get; set; } = string.Empty;
        public string MetaObjectKindTitle { get; set; } = string.Empty;
        public Guid MetaObjectKindUid { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Memo { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}