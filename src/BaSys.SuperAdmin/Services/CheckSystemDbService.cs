using BaSys.Common.Enums;
using BaSys.Common.Infrastructure;
using BaSys.SuperAdmin.Abstractions;
using BaSys.SuperAdmin.Data;
using BaSys.SuperAdmin.Data.Models;
using BaSys.SuperAdmin.Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BaSys.SuperAdmin.Services;

public class CheckSystemDbService : ICheckSystemDbService
{ 
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SuperAdminDbContext _context;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly InitAppSettings? _initAppSettings;

    public CheckSystemDbService(IConfiguration configuration,
        UserManager<IdentityUser> userManager,
        SuperAdminDbContext context,
        RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _context = context;
        _roleManager = roleManager;
        
        _initAppSettings = configuration.GetSection("InitAppSettings").Get<InitAppSettings>();
    }
    
    public async Task CheckDbs()
    {
        var appId = await CheckCurrentApp();
        await CheckMainDb(appId);
        await CheckSa();
    }

    #region private methods
    private async Task<string> CheckCurrentApp()
    {
        var currentApp = _initAppSettings?.CurrentApp;
        if (currentApp == null)
            throw new ApplicationException("InitAppSettings:CurrentApp is not set in the config!");

        if (string.IsNullOrEmpty(currentApp.Id))
            throw new ApplicationException("InitAppSettings:CurrentApp:Id is not set in the config!");
        if (string.IsNullOrEmpty(currentApp.Title))
            throw new ApplicationException("InitAppSettings:CurrentApp:Title is not set in the config!");

        if (!await _context.AppRecords.AnyAsync(x => x.Id.ToUpper() == currentApp.Id.ToUpper()))
        {
            _context.AppRecords.Add(new AppRecord
            {
                Id = currentApp.Id,
                Title = currentApp.Title
            });

            await _context.SaveChangesAsync();
        }

        return currentApp.Id;
    }
    
    private async Task CheckMainDb(string appId)
    {
        var mainDb = _initAppSettings?.MainDb;
        if (mainDb == null)
            throw new ApplicationException("InitAppSettings:MainDb is not set in the config!");
        
        if (string.IsNullOrEmpty(mainDb.Title))
            throw new ApplicationException("InitAppSettings:MainDb:Title is not set in the config!");
        if (mainDb.DbKind == null)
            throw new ApplicationException("InitAppSettings:MainDb:DbKind is not set in the config!");
        if (string.IsNullOrEmpty(mainDb.ConnectionString))
            throw new ApplicationException("InitAppSettings:MainDb:ConnectionString is not set in the config!");
        
        var isInfoRecordExists = await _context.DbInfoRecords
            .AnyAsync(x =>
                x.AppId.ToLower() == appId.ToLower() &&
                x.DbKind == mainDb.DbKind &&
                x.Title.ToLower() == mainDb.Title.ToLower());

        if (!isInfoRecordExists)
        {
            _context.DbInfoRecords.Add(new DbInfoRecord
            {
                AppId = appId,
                Title = mainDb.Title,
                DbKind = mainDb.DbKind.Value,
                ConnectionString = mainDb.ConnectionString
            });
            
            await _context.SaveChangesAsync();
        }
        else
        {
            var infoRecord = await _context.DbInfoRecords
                .FirstAsync(x =>
                    x.AppId.ToLower() == appId.ToLower() &&
                    x.DbKind == mainDb.DbKind &&
                    x.Title.ToLower() == mainDb.Title.ToLower());

            if (infoRecord.ConnectionString.ToLower() != mainDb.ConnectionString.ToLower())
            {
                infoRecord.ConnectionString = mainDb.ConnectionString;
                await _context.SaveChangesAsync();
            }
        }
    }
    
    private async Task CheckSa()
    {
        var sa = _initAppSettings?.Sa;
        if (sa == null)
            throw new ApplicationException("InitAppSettings:Sa is not set in the config!");
            
        if (string.IsNullOrEmpty(sa.Login))
            throw new ApplicationException("InitAppSettings:Sa:Login is not set in the config!");
        if (string.IsNullOrEmpty(sa.Password))
            throw new ApplicationException("InitAppSettings:Sa:Password is not set in the config!");
        if (sa.DbKind == null)
            throw new ApplicationException("InitAppSettings:Sa:DbKind is not set in the config!");
        if (string.IsNullOrEmpty(sa.ConnectionString))
            throw new ApplicationException("InitAppSettings:Sa:ConnectionString is not set in the config!");

        if (!_userManager.Users.Any(x => x.Email != null && x.Email.ToUpper() == sa.Login.ToUpper()))
        {
            // create user
            await _userManager.CreateAsync(new IdentityUser
            {
                UserName = sa.Login,
                Email = sa.Login
            }, sa.Password);
            var saUser = await _userManager.FindByEmailAsync(sa.Login);

            // create role
            if (!_roleManager.Roles.Any(x => x.Name != null && x.Name.ToLower() == ApplicationRole.SuperAdministrator.ToLower()))
            {
                await _roleManager.CreateAsync(new IdentityRole(ApplicationRole.SuperAdministrator));
                if (saUser != null)
                    await _userManager.AddToRoleAsync(saUser, ApplicationRole.SuperAdministrator);
            }
        }
    }
    #endregion
}