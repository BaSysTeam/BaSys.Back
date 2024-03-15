using BaSys.Common.Infrastructure;
using BaSys.Host.Abstractions;
using BaSys.SuperAdmin.Data.Identity;
using BaSys.SuperAdmin.Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BaSys.Host.Services;

public class MainDbCheckService : IMainDbCheckService
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    
    public MainDbCheckService(UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }
    
    public async Task Check(InitAppSettings initAppSettings)
    {
        await CheckRoles();
        var adminLogin = await CheckAdminUser(initAppSettings);
        await CheckAdminRoles(adminLogin);
    }

    private async Task CheckAdminRoles(string? adminLogin)
    {
        if (!string.IsNullOrEmpty(adminLogin))
        {
            var adminUser = await _userManager.FindByEmailAsync(adminLogin);
            if (adminUser == null)
                return;
            
            var adminRoles = new[]
            {
                ApplicationRole.Administrator,
                ApplicationRole.User,
                ApplicationRole.Designer
            };

            foreach (var adminRole in adminRoles)
            {
                if (!await _userManager.IsInRoleAsync(adminUser, adminRole))
                    await _userManager.AddToRoleAsync(adminUser, adminRole);
            }
        }
    }

    private async Task<string?> CheckAdminUser(InitAppSettings initAppSettings)
    {
        var isAdminSa = string.IsNullOrEmpty(initAppSettings.MainDb?.AdminLogin) ||
                        string.IsNullOrEmpty(initAppSettings.MainDb?.AdminPassword);

        var adminLogin = isAdminSa ? initAppSettings.Sa?.Login : initAppSettings.MainDb?.AdminLogin;
        var adminPassword = isAdminSa ? initAppSettings.Sa?.Password : initAppSettings.MainDb?.AdminPassword;
        
        if (!string.IsNullOrEmpty(adminLogin) && !string.IsNullOrEmpty(adminPassword))
        {
            if (!await _userManager.Users.AnyAsync(x => x.NormalizedEmail == adminLogin.ToUpper()))
            {
                await _userManager.CreateAsync(new IdentityUser()
                {
                    UserName = adminLogin,
                    Email = adminLogin
                }, adminPassword);
            }
        }

        return adminLogin;
    }

    private async Task CheckRoles()
    {
        foreach (var role in ApplicationRole.AllApplicationRoles())
        {
            if (!await _roleManager.RoleExistsAsync(role.Name))
                await _roleManager.CreateAsync(new SaDbRole(role.Name));
        }
    }
}