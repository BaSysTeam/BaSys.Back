using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.Host.DAL.Abstractions
{
    public interface IMetaObjectTableCommand
    {
        Guid TableUid { get; set; }
        string TableName { get; set; }
    }
}
