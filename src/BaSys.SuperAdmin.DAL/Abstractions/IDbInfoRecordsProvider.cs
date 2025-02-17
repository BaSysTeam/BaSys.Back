using BaSys.SuperAdmin.DAL.Models;

namespace BaSys.SuperAdmin.DAL.Abstractions;

public interface IDbInfoRecordsProvider
{
    Task Update();
    List<DbInfoRecord> GetCollection();
    List<DbInfoRecord> GetActiveRecords();
    DbInfoRecord? GetDbInfoRecordByDbName(string dbName);
}