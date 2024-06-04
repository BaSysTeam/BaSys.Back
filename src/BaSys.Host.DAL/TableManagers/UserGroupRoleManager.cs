using System.Data;
using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.ModelConfigurations;

namespace BaSys.Host.DAL.TableManagers;

public class UserGroupRoleManager : TableManagerBase
{
    public UserGroupRoleManager(IDbConnection connection) : base(connection, new UserGroupRoleConfiguration())
    {
    }
}