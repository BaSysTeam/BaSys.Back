using BaSys.Host.Abstractions;
using BaSys.Host.DAL;
using BaSys.SuperAdmin.Abstractions;
using BaSys.SuperAdmin.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace BaSys.Host.Services;

/// <summary>
/// Creates tables, users and roles when DB created. Uses EF DbContext.
/// </summary>
public class WorkDbService : IWorkDbService
{
    // private readonly IServiceProvider _serviceProvider;
    private readonly IMainDbCheckService _mainDbCheckService;
    private readonly IConfiguration _configuration;

    public WorkDbService(IMainDbCheckService mainDbCheckService,
        IConfiguration configuration)
    {
        // _serviceProvider = serviceProvider;
        _mainDbCheckService = mainDbCheckService;
        _configuration = configuration;
    }

    public async Task<bool> InitWorkDb(string adminLogin, string adminPassword, string? dbName = null)
    {
        try
        {
            var initAppSettings = _configuration.GetSection("InitAppSettings").Get<InitAppSettings>();
            if (initAppSettings?.MainDb == null)
                return false;

            initAppSettings.MainDb.AdminLogin = adminLogin;
            initAppSettings.MainDb.AdminPassword = adminPassword;
            
            // get context for service with the required connection from request parameter in DI
            await _mainDbCheckService.Check(initAppSettings, dbName);
        }
        catch
        {
            return false;
        }
        
        return true;
    }
}