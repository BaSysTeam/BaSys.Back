﻿using System;
using System.Collections.Generic;
using System.Text;

namespace BaSys.Metadata.Models
{
    public class MetadataTreeNode
    {
        public string Key { get; set; }
        public string Label { get; set; }
        public string Icon { get; set; }
        public List<MetadataTreeNode> Children { get; set; }
    }
}