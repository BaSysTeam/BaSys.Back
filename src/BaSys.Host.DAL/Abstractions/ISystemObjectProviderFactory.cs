using BaSys.Host.DAL.DataProviders;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.Host.DAL.Abstractions
{
    public interface ISystemObjectProviderFactory
    {
        void SetUp(IDbConnection connection);
        T Create<T>() where T : class;
        MetaObjectStorableProvider CreateMetaObjectStorableProvider(string kindNamePlural);
    }

}
