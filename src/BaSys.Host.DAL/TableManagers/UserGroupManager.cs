using System.Data;
using BaSys.FluentQueries.Abstractions;
using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.ModelConfigurations;

namespace BaSys.Host.DAL.TableManagers;

public class UserGroupManager : TableManagerBase
{
    public UserGroupManager(IDbConnection connection) : base(connection, new UserGroupConfiguration())
    {
    }
}