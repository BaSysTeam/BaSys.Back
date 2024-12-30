using BaSys.Host.DAL.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.Host.DAL.Helpers
{
    public sealed class DropColumnCommand: IMetaObjectChangeCommand
    {
        public Guid TableUid { get; set; }
        public string TableName { get; set; } = string.Empty;
        public string ColumnName { get; set; } = string.Empty;
    }
}
