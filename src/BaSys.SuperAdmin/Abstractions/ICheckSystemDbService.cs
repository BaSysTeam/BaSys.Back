using BaSys.SuperAdmin.Infrastructure.Models;

namespace BaSys.SuperAdmin.Abstractions;

public interface ICheckSystemDbService
{
    Task CheckDbs();
    public event Action<InitAppSettings> CheckAdminRolesEvent;
}