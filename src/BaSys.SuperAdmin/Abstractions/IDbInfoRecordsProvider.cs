using BaSys.SuperAdmin.DAL.Models;

namespace BaSys.SuperAdmin.Abstractions;

public interface IDbInfoRecordsProvider
{
    Task Update();
    DbInfoRecord? GetDbInfoRecordByDbName(string dbName);
}