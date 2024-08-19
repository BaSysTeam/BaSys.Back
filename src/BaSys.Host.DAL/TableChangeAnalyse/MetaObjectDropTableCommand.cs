using BaSys.Host.DAL.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.Host.DAL.TableChangeAnalyse
{
    public sealed class MetaObjectDropTableCommand: IMetaObjectChangeCommand
    {
        public Guid TableUid { get; set; }
        public string TableName { get; set; } = string.Empty;
    }
}
