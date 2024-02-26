using BaSys.SuperAdmin.Data.Models;

namespace BaSys.SuperAdmin.Abstractions;

public interface IAppRecordsService
{
    Task<IEnumerable<AppRecord>> GetAppRecords();
    Task<AppRecord> GetAppRecord(string id);
    Task<AppRecord> AddAppRecord(AppRecord appRecord);
    Task<int> DeleteAppRecord(string id);
    Task<AppRecord> UpdateAppRecord(AppRecord appRecord);
}