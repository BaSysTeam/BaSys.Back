using BaSys.SuperAdmin.DAL.Models;
using BaSys.SuperAdmin.DTO;

namespace BaSys.SuperAdmin.Abstractions;

public interface IAppRecordsService
{
    Task<IEnumerable<AppRecordDto>> GetAppRecords();
    Task<AppRecordDto?> GetAppRecord(string id);
    Task<AppRecordDto> AddAppRecord(AppRecordDto appRecord);
    Task<int> DeleteAppRecord(string id);
    Task<AppRecordDto> UpdateAppRecord(AppRecordDto appRecord);
}