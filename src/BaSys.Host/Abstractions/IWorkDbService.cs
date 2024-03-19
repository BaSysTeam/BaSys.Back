namespace BaSys.Host.Abstractions;

public interface IWorkDbService
{
    Task<bool> InitDb(int dbInfoRecordId);
}