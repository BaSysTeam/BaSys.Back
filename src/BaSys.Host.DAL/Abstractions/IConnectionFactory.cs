using BaSys.Common.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.Host.DAL.Abstractions
{
    public interface IConnectionFactory
    {
        IDbConnection CreateConnection(string connectionString, DbKinds dbKind);
    }
}
