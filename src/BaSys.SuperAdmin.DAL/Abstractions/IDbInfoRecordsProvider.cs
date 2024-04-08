using BaSys.SuperAdmin.DAL.Models;

namespace BaSys.SuperAdmin.DAL.Abstractions;

public interface IDbInfoRecordsProvider
{
    Task Update();
    DbInfoRecord? GetDbInfoRecordByDbName(string dbName);
}