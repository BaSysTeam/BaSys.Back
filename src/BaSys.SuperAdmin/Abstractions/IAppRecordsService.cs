using BaSys.SuperAdmin.Data.Models;

namespace BaSys.SuperAdmin.Abstractions;

public interface IAppRecordsService
{
    Task<IEnumerable<AppRecord>> GetAppRecords();
    Task<AppRecord> AddAppRecord(AppRecord appRecord);
    Task<bool> DeleteAppRecord(string appRecordId);
    Task<AppRecord> EditAppRecord(AppRecord appRecord);
}