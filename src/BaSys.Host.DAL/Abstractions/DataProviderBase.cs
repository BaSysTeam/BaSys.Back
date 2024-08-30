using BaSys.FluentQueries.Abstractions;
using BaSys.FluentQueries.Enums;
using BaSys.Host.DAL.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.Host.DAL.Abstractions
{
    public abstract class DataProviderBase
    {
        protected IDbConnection _connection;
        protected SqlDialectKinds _sqlDialect;

        protected IQuery? _query;

        public IQuery? LastQuery => _query;

        protected DataProviderBase(IDbConnection connection)
        {
            _connection = connection;
            _sqlDialect = SqlDialectKindHelper.GetDialectKind(connection);
        }

    }
}
