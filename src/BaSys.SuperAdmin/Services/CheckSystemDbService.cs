using BaSys.Common.Enums;
using BaSys.Common.Infrastructure;
using BaSys.SuperAdmin.Abstractions;
using BaSys.SuperAdmin.Data;
using BaSys.SuperAdmin.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BaSys.SuperAdmin.Services;

public class CheckSystemDbService : ICheckSystemDbService
{ 
    private readonly IConfiguration _configuration;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SuperAdminDbContext _context;
    private readonly RoleManager<IdentityRole> _roleManager;

    public CheckSystemDbService(IConfiguration configuration,
        UserManager<IdentityUser> userManager,
        SuperAdminDbContext context,
        RoleManager<IdentityRole> roleManager)
    {
        _configuration = configuration;
        _userManager = userManager;
        _context = context;
        _roleManager = roleManager;
    }
    
    public async Task CheckSystemDb()
    {
        var appId = await CheckApp();
        await CheckDbInfo(appId);
        await CheckSaUser();
    }

    #region private methods
    private async Task CheckDbInfo(string appId)
    {
        var dbTitle = _configuration["MainDb:Title"];
        var dbKind = _configuration["MainDb:DbKind"];
        var dbConnStr = _configuration["MainDb:ConnectionString"];
        
        if (string.IsNullOrEmpty(dbTitle))
            throw new ApplicationException("MainDb.Title is not set in the config!");
        if (string.IsNullOrEmpty(dbKind) || !int.TryParse(dbKind, out var kind))
            throw new ApplicationException("MainDb.DbKind is not set in the config!");
        if (string.IsNullOrEmpty(dbConnStr))
            throw new ApplicationException("MainDb.ConnectionString is not set in the config!");
        
        if (await _context.DbInfoRecords.AnyAsync(x => x.AppId.ToLower() == appId.ToLower() &&
                                                       (int)x.DbKind == kind &&
                                                       x.ConnectionString.ToLower() == dbConnStr.ToLower()))
        return;

        _context.DbInfoRecords.Add(new DbInfoRecord
        {
            AppId = appId,
            Title = dbTitle,
            DbKind = (DbKinds) kind,
            ConnectionString = dbConnStr
        });

        await _context.SaveChangesAsync();
    }
    
    private async Task CheckSaUser()
    {
        var saLogin = _configuration["Sa:Login"];
        var saPassword = _configuration["Sa:Password"];
        
        if (string.IsNullOrEmpty(saLogin))
            throw new ApplicationException("Sa.Login is not set in the config!");
        if (string.IsNullOrEmpty(saPassword))
            throw new ApplicationException("Sa.Password is not set in the config!");

        if (!_userManager.Users.Any(x => x.Email != null && x.Email.ToUpper() == saLogin.ToUpper()))
        {
            // create user
            await _userManager.CreateAsync(new IdentityUser
            {
                UserName = saLogin,
                Email = saLogin
            }, saPassword);
            var saUser = await _userManager.FindByEmailAsync(saLogin); 

            // create role
            if (!_roleManager.Roles.Any(x => x.Name != null && x.Name.ToLower() == ApplicationRole.SuperAdministrator.ToLower()))
            {
                await _roleManager.CreateAsync(new IdentityRole(ApplicationRole.SuperAdministrator));
                if (saUser != null)
                    await _userManager.AddToRoleAsync(saUser, ApplicationRole.SuperAdministrator);
            }
        }
    }

    private async Task<string> CheckApp()
    {
        var appId = _configuration["CurrentApp:Id"];
        var appTitle = _configuration["CurrentApp:Title"];

        if (string.IsNullOrEmpty(appId))
            throw new ApplicationException("CurrentApp.Id is not set in the config!");
        if (string.IsNullOrEmpty(appTitle))
            throw new ApplicationException("CurrentApp.Title is not set in the config!");

        if (!await _context.AppRecords.AnyAsync(x => x.Id.ToUpper() == appId.ToUpper()))
        {
            _context.AppRecords.Add(new AppRecord
            {
                Id = appId,
                Title = appTitle
            });

            await _context.SaveChangesAsync();
        }

        return appId;
    }
    #endregion
}