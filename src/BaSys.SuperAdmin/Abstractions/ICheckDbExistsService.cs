using BaSys.SuperAdmin.DTO;

namespace BaSys.SuperAdmin.Abstractions;

public interface ICheckDbExistsService
{
    Task<bool?> IsExists(DbInfoRecordDto dbInfoRecord);
}