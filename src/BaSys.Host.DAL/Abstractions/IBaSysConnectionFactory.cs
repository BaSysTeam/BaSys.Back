using BaSys.Common.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.Host.DAL.Abstractions
{
    /// <summary>
    /// Provides a factory to create database connection.
    /// </summary>
    public interface IBaSysConnectionFactory
    {
        /// <summary>
        /// Creates connection to database.
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="dbKind"></param>
        /// <returns></returns>
        IDbConnection CreateConnection(string connectionString, DbKinds dbKind);
    }
}
