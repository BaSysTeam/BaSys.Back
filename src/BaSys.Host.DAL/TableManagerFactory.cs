using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.TableManagers;
using BaSys.Metadata.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.Host.DAL
{
    public sealed class TableManagerFactory: ITableManagerFactory
    {
        private IDbConnection? _connection;
    
        public void SetUp(IDbConnection connection)
        {
            _connection = connection;
        }

        public T Create<T>() where T :class, ITableManager
        {
            if (_connection == null)
                throw new ArgumentNullException(nameof(_connection));

            var type = typeof(T);
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (!typeof(ITableManager).IsAssignableFrom(type))
                throw new ArgumentException("The type must implement ITableManager interface", nameof(type));

            var instance = Activator.CreateInstance(typeof(T), _connection) as T;

            return instance;

        }

        public MetaObjectManager CreateMetaObjectManager(string kindNamePlural)
        {
            if (_connection == null)
                throw new ArgumentNullException(nameof(_connection));

            return new MetaObjectManager(_connection, kindNamePlural);
        }

        public DataObjectManager CreateDataObjectManager(MetaObjectKindSettings kindSettings, 
            MetaObjectStorableSettings objectSettingns, 
            PrimitiveDataTypes primitiveDataTypes)
        {
            if (_connection == null)
                throw new ArgumentNullException(nameof(_connection));

            return new DataObjectManager(_connection, kindSettings, objectSettingns, primitiveDataTypes);
        }
    }
}
