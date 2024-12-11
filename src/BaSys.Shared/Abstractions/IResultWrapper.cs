using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.Common.Abstractions
{
    public interface IResultWrapper
    {
        void Error(int status, string message, string info = null);
    }
}
