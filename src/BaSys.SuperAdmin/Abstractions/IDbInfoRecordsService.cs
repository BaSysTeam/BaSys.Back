using System.Collections;
using BaSys.SuperAdmin.Data.Models;

namespace BaSys.SuperAdmin.Abstractions;

public interface IDbInfoRecordsService
{
    Task<IEnumerable<DbInfoRecord>> GetDbInfoRecordsByAppId(string appId);
    Task<DbInfoRecord> AddDbInfoRecord(DbInfoRecord dbInfoRecord);
    Task<DbInfoRecord> EditDbInfoRecord(DbInfoRecord dbInfoRecord);
    Task<int> DeleteDbInfoRecord(int dbInfoRecordId);
}