using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.Common.Abstractions
{
    public interface ISystemObject
    {
        Guid Uid { get; set; }
        void BeforeSave();
    }
}
