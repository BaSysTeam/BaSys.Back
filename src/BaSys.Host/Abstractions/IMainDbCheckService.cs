using BaSys.SuperAdmin.Infrastructure.Models;

namespace BaSys.Host.Abstractions;

public interface IMainDbCheckService
{
    Task Check(InitAppSettings initAppSettings, string? dbName = null);
}