﻿using BaSys.FluentQueries.Models;
using BaSys.Metadata.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.Host.DAL.ModelConfigurations
{
    public sealed class MetadataGroupConfiguration: DataModelConfiguration<MetadataGroup>
    {
        public MetadataGroupConfiguration()
        {
            Table("sys_metadata_groups");

            Column("uid").IsPrimaryKey();
            Column("title").MaxLength(100);
            Column("iconclass").MaxLength(20).IsOptional();
            Column("memo").MaxLength(300).IsOptional();
        }
    }
}
