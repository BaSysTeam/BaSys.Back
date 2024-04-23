using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.ModelConfigurations;
using BaSys.Metadata.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.Host.DAL.TableManagers
{
    public sealed class MetaObjectManager: TableManagerBase
    {
        public MetaObjectManager(IDbConnection connection, MetadataKind kind):base(connection, new MetadataObjectConfiguration())
        {
            _tableName = $"_sys_meta_{kind.Name}";
        }

    }
}
