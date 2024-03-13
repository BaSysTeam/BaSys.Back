using BaSys.Host.Data;

namespace BaSys.Host.DAL;

public interface IContextFactory
{
    ApplicationDbContext? GetContext();
}