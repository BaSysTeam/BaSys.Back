using System.Data;

namespace BaSys.Host.Abstractions
{
    public interface IDbInitService
    {
        void  SetUp(IDbConnection connection);
        Task ExecuteAsync();
    }
}
