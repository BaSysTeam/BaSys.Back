using System.Collections;
using BaSys.SuperAdmin.DAL.Models;
using BaSys.SuperAdmin.DTO;

namespace BaSys.SuperAdmin.Abstractions;

public interface IDbInfoRecordsService
{
    Task<IEnumerable<DbInfoRecordDto>> GetDbInfoRecords();
    Task<DbInfoRecordDto?> GetDbInfoRecord(int id);
    Task<IEnumerable<DbInfoRecordDto>> GetDbInfoRecordsByAppId(string appId);
    Task<DbInfoRecordDto> AddDbInfoRecord(DbInfoRecordDto dbInfoRecord);
    Task<DbInfoRecordDto> EditDbInfoRecord(DbInfoRecordDto dbInfoRecord);
    Task<int> DeleteDbInfoRecord(int dbInfoRecordId);
    Task<DbInfoRecordDto> SwitchActivityDbInfoRecord(int dbInfoRecordId);
}