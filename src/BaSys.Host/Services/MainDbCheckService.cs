﻿using System.Security.Claims;
using BaSys.Common.Infrastructure;
using BaSys.Host.Abstractions;
using BaSys.Host.DAL;
using BaSys.Host.Identity;
using BaSys.Host.Identity.Models;
using BaSys.SuperAdmin.Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BaSys.Host.Services;

public class MainDbCheckService : IMainDbCheckService
{
    private readonly UserManager<WorkDbUser> _userManager;
    private readonly RoleManager<WorkDbRole> _roleManager;
    private readonly ApplicationDbContext _context;
    
    public MainDbCheckService(UserManager<WorkDbUser> userManager,
        RoleManager<WorkDbRole> roleManager,
        ApplicationDbContext context)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _context = context;
    }
    
    public async Task Check(InitAppSettings initAppSettings, string? dbName = null)
    {
        _context.Database.Migrate();
        
        await CheckRoles();
        var adminLogin = await CheckAdminUser(initAppSettings, dbName);
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

    private async Task<string?> CheckAdminUser(InitAppSettings initAppSettings, string? dbName = null)
    {
        var isAdminSa = string.IsNullOrEmpty(initAppSettings.MainDb?.AdminLogin) ||
                        string.IsNullOrEmpty(initAppSettings.MainDb?.AdminPassword);

        var adminLogin = isAdminSa ? initAppSettings.Sa?.Login : initAppSettings.MainDb?.AdminLogin;
        var adminPassword = isAdminSa ? initAppSettings.Sa?.Password : initAppSettings.MainDb?.AdminPassword;
        
        if (!string.IsNullOrEmpty(adminLogin) && !string.IsNullOrEmpty(adminPassword))
        {
            if (!await _userManager.Users.AnyAsync(x => x.Email != null && x.Email.ToUpper() == adminLogin.ToUpper()))
            {
                await _userManager.CreateAsync(new WorkDbUser()
                {
                    UserName = adminLogin,
                    Email = adminLogin
                }, adminPassword);
            }

            // add DbName claim
            Claim? dbNameClaim = null;
            var currentUser = await _userManager.Users.FirstAsync(x => x.Email != null && x.Email.ToUpper() == adminLogin.ToUpper());
            if (string.IsNullOrEmpty(dbName))
            {
                var claims = await _userManager.GetClaimsAsync(currentUser);
                if (!claims.Any(c => c.Type == "DbName") && !string.IsNullOrEmpty(initAppSettings.MainDb?.Name))
                    dbNameClaim = new Claim("DbName", initAppSettings.MainDb.Name);
            }
            else
            {
                dbNameClaim = new Claim("DbName", dbName);
            }
            
            if (dbNameClaim != null)
                await _userManager.AddClaimAsync(currentUser, dbNameClaim);
        }

        return adminLogin;
    }

    private async Task CheckRoles()
    {
        foreach (var role in ApplicationRole.AllApplicationRoles())
        {
            if (!await _roleManager.RoleExistsAsync(role.Name))
                await _roleManager.CreateAsync(new WorkDbRole(role.Name));
        }
    }
}