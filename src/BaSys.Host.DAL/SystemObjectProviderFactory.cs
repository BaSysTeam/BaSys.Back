using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.DataProviders;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace BaSys.Host.DAL
{
    public sealed class SystemObjectProviderFactory : ISystemObjectProviderFactory
    {
        private IDbConnection? _connection;

        public void SetUp(IDbConnection connection)
        {
            _connection = connection;
        }

        public T Create<T>() where T : class
        {
            if (_connection == null)
                throw new ArgumentNullException(nameof(_connection));

            var type = typeof(T);
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            // Ensure T is a class type that implements ISystemObjectProvider<> for some type
            var providerInterface = typeof(T).GetInterfaces()
                .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ISystemObjectProvider<>));

            if (providerInterface == null)
            {
                throw new ArgumentException("The type must implement ISystemObjectProvider<> interface with a specific type parameter", nameof(T));
            }

            var instance = Activator.CreateInstance(typeof(T), _connection) as T;

            return instance;

        }

        public MetaObjectStorableProvider CreateMetaObjectStorableProvider(string kindNamePlural)
        {
            if (_connection == null)
                throw new ArgumentNullException(nameof(_connection));

            return new MetaObjectStorableProvider(_connection, kindNamePlural);
        }
    }
}
