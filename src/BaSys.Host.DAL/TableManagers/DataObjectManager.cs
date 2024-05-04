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
    public sealed class DataObjectManager : TableManagerBase
    {
        public DataObjectManager(IDbConnection connection,
            MetaObjectKindSettings kindSettings,
            MetaObjectStorableSettings objectSettings,
            PrimitiveDataTypes primitiveDataTypes) :

            base(connection, new DataObjectConfiguration(kindSettings,
                objectSettings,
                primitiveDataTypes))
        {

        }
    }
}
