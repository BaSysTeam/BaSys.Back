using BaSys.Common.Enums;
using BaSys.FluentQueries.Enums;
using Microsoft.Data.SqlClient;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.Host.DAL
{
    public sealed class ConnectionFactory
    {
        public IDbConnection CreateConnection(string connectionString, DbKinds dbKind)
        {
            IDbConnection connection = null;
            switch (dbKind)
            {
                case DbKinds.MsSql:
                    connection = new SqlConnection(connectionString);
                    break;
                case DbKinds.PgSql:
                    connection = new NpgsqlConnection(connectionString);
                    break;
                default:
                    throw new NotImplementedException($"DbKind {dbKind} not supported");
            }

            return connection;
        }
    }
}
