using BaSys.Common.Infrastructure;
using BaSys.Host.Abstractions;
using BaSys.Host.Data;
using BaSys.SuperAdmin.Data.Identity;
using BaSys.SuperAdmin.Infrastructure.Models;
using Microsoft.AspNetCore.Identity;

namespace BaSys.Host.Services;

public class MainDbCheckService : IMainDbCheckService
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ApplicationDbContext _context;
    
    public MainDbCheckService(UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager,
        ApplicationDbContext context)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _context = context;
    }
    
    public async Task Check(InitAppSettings initAppSettings)
    {
        // Check roles
        foreach (var role in ApplicationRole.AllApplicationRoles())
        {
            if (!await _roleManager.RoleExistsAsync(role.Title))
                await _roleManager.CreateAsync(new SaDbRole(role.Title));
        }
        
        // Check admin
        var isAdminSa = string.IsNullOrEmpty(initAppSettings.MainDb?.AdminLogin) ||
                        string.IsNullOrEmpty(initAppSettings.MainDb?.AdminPassword);


        // Check admin roles
        // ...
    }
}