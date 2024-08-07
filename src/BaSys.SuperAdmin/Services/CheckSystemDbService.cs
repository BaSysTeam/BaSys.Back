﻿using BaSys.Common.Infrastructure;
using BaSys.SuperAdmin.Abstractions;
using BaSys.SuperAdmin.DAL;
using BaSys.SuperAdmin.DAL.Models;
using BaSys.SuperAdmin.Data;
using BaSys.SuperAdmin.Data.Identity;
using BaSys.SuperAdmin.Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BaSys.SuperAdmin.Services;

public class CheckSystemDbService : ICheckSystemDbService
{ 
    private readonly UserManager<SaDbUser> _saUserManager;
    private readonly RoleManager<SaDbRole> _saRoleManager;
    private readonly SuperAdminDbContext _saContext;
    private readonly InitAppSettings? _initAppSettings;

    public event Action<InitAppSettings>? CheckAdminRolesEvent;

    public CheckSystemDbService(IConfiguration configuration,
        UserManager<SaDbUser> saUserManager,
        RoleManager<SaDbRole> saRoleManager,
        SuperAdminDbContext saContext)
    {
        _saUserManager = saUserManager;
        _saContext = saContext;
        _saRoleManager = saRoleManager;
        
        _initAppSettings = configuration.GetSection("InitAppSettings").Get<InitAppSettings>();
        if (_initAppSettings == null)
            throw new ApplicationException("InitAppSettings is not set in the config!");
    }
    
    public async Task CheckDbs()
    {
        var appId = await CheckCurrentApp();
        await CheckSa();
        await CheckMainDb(appId);
        await CheckAdminRoles();
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

        _saContext.Database.Migrate();
        
        if (!await _saContext.AppRecords.AnyAsync(x => x.Id.ToUpper() == currentApp.Id.ToUpper()))
        {
            _saContext.AppRecords.Add(new AppRecord
            {
                Id = currentApp.Id,
                Title = currentApp.Title
            });

            await _saContext.SaveChangesAsync();
        }

        return currentApp.Id;
    }
    
    private async Task CheckMainDb(string appId)
    {
        var mainDb = _initAppSettings?.MainDb;
        if (mainDb == null)
            throw new ApplicationException("InitAppSettings:MainDb is not set in the config!");
        
        if (string.IsNullOrEmpty(mainDb.Name))
            throw new ApplicationException("InitAppSettings:MainDb:Name is not set in the config!");
        if (mainDb.DbKind == null)
            throw new ApplicationException("InitAppSettings:MainDb:DbKind is not set in the config!");
        if (string.IsNullOrEmpty(mainDb.ConnectionString))
            throw new ApplicationException("InitAppSettings:MainDb:ConnectionString is not set in the config!");
        
        var isInfoRecordExists = await _saContext.DbInfoRecords
            .AnyAsync(x =>
                x.AppId.ToLower() == appId.ToLower() &&
                x.DbKind == mainDb.DbKind &&
                x.Name.ToLower() == mainDb.Name.ToLower());
        
        if (!isInfoRecordExists)
        {
            _saContext.DbInfoRecords.Add(new DbInfoRecord
            {
                AppId = appId,
                Name = mainDb.Name,
                DbKind = mainDb.DbKind.Value,
                ConnectionString = mainDb.ConnectionString
            });
            
            await _saContext.SaveChangesAsync();
        }
        else
        {
            var infoRecord = await _saContext.DbInfoRecords
                .FirstAsync(x =>
                    x.AppId.ToLower() == appId.ToLower() &&
                    x.DbKind == mainDb.DbKind &&
                    x.Name.ToLower() == mainDb.Name.ToLower());
        
            if (infoRecord.ConnectionString.ToLower() != mainDb.ConnectionString.ToLower())
            {
                infoRecord.ConnectionString = mainDb.ConnectionString;
                await _saContext.SaveChangesAsync();
            }
        }
    }
    
    private async Task CheckSa()
    {
        var saSettings = _initAppSettings?.Sa;
        if (saSettings == null)
            throw new ApplicationException("InitAppSettings:Sa is not set in the config!");
            
        if (string.IsNullOrEmpty(saSettings.Login))
            throw new ApplicationException("InitAppSettings:Sa:Login is not set in the config!");
        if (string.IsNullOrEmpty(saSettings.Password))
            throw new ApplicationException("InitAppSettings:Sa:Password is not set in the config!");
        if (saSettings.DbKind == null)
            throw new ApplicationException("InitAppSettings:Sa:DbKind is not set in the config!");
        if (string.IsNullOrEmpty(saSettings.ConnectionString))
            throw new ApplicationException("InitAppSettings:Sa:ConnectionString is not set in the config!");
        
        if (!_saUserManager.Users.Any(x => x.Email != null && x.Email.ToUpper() == saSettings.Login.ToUpper()))
        {
            // create user
            await _saUserManager.CreateAsync(new SaDbUser()
            {
                UserName = saSettings.Login,
                Email = saSettings.Login
            }, saSettings.Password);
            var saUser = await _saUserManager.FindByEmailAsync(saSettings.Login);

            // create role
            if (!_saRoleManager.Roles.Any(x => x.Name != null && x.Name.ToLower() == ApplicationRole.SuperAdministrator.ToLower()))
            {
                await _saRoleManager.CreateAsync(new SaDbRole(ApplicationRole.SuperAdministrator));
                if (saUser != null)
                    await _saUserManager.AddToRoleAsync(saUser, ApplicationRole.SuperAdministrator);
            }
        }
    }

    private async Task CheckAdminRoles()
    {
        CheckAdminRolesEvent?.Invoke(_initAppSettings!);
    }
    #endregion
}