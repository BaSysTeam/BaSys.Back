using BaSys.FluentQueries.QueryBuilders;
using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.ModelConfigurations;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.Host.DAL.TableManagers
{
    public sealed class MetaObjectKindManager: TableManagerBase
    {
        public MetaObjectKindManager(IDbConnection connection):base(connection, new MetaObjectKindConfiguration())
        {
            
        }
    }
}
