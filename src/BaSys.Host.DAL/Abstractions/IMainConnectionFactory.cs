using System.Data;

namespace BaSys.Host.DAL.Abstractions;

public interface IMainConnectionFactory
{
    public IDbConnection CreateConnection();
}