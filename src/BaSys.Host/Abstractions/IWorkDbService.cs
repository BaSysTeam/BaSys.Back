namespace BaSys.Host.Abstractions;

public interface IWorkDbService
{
    Task<bool> InitWorkDb();
}