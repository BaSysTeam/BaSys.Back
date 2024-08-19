using BaSys.Host.DAL.Abstractions;
using BaSys.Metadata.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.Host.DAL.Helpers
{
    public sealed class MetaObjectTableAddColumnCommand: IMetaObjectChangeCommand
    {
        public Guid TableUid { get; set; }
        public string TableName { get; set; } = string.Empty;
        public MetaObjectTableColumn Column { get; set; }
    }
}
