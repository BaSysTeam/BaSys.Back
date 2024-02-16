namespace BaSys.Host.Data;

public interface IContextFactory
{
    ApplicationDbContext? GetContext();
}