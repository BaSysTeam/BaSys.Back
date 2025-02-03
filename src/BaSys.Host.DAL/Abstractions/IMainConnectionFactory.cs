using BaSys.Common.Enums;
using System.Data;

namespace BaSys.Host.DAL.Abstractions;

public interface IMainConnectionFactory
{
    public IDbConnection CreateConnection();
    IDbConnection CreateConnection(string connectionString, DbKinds dbKind);
}