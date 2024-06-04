using System.Data;
using BaSys.FluentQueries.Abstractions;
using BaSys.FluentQueries.QueryBuilders;
using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.ModelConfigurations;
using Dapper;

namespace BaSys.Host.DAL.TableManagers;

public sealed class MigrationManager : TableManagerBase
{
    public MigrationManager(IDbConnection connection) : base(connection, new MigrationsConfiguration())
    {
    }
    
}