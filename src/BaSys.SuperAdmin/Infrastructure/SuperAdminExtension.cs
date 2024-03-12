using BaSys.SuperAdmin.Abstractions;
using BaSys.SuperAdmin.Controllers;
using BaSys.SuperAdmin.Data;
using BaSys.SuperAdmin.Data.MsSqlContext;
using BaSys.SuperAdmin.Infrastructure.Models;
using BaSys.SuperAdmin.Services;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.EntityFrameworkCore;

namespace BaSys.SuperAdmin.Infrastructure;

public static class SuperAdminExtension
{
    public static IServiceCollection AddSuperAdmin(this IServiceCollection services, IConfigurationSection configurationSection)
    {
        var initAppSettings = configurationSection.Get<InitAppSettings>();
        if (initAppSettings == null)
            throw new ApplicationException("InitAppSettings is not set in the config!");
        if (string.IsNullOrEmpty(initAppSettings.Sa?.ConnectionString))
            throw new ApplicationException("InitAppSettings:Sa:ConnectionString is not set in the config!");

        var connectionString = initAppSettings.Sa.ConnectionString;
        // add db context
        services.AddDbContext<MsSqlSuperAdminDbContext>(options => options.UseSqlServer(connectionString));
        
        // add controllers
        services.AddControllers()
            .PartManager
            .ApplicationParts
            .Add(new AssemblyPart(typeof(SuperAdminExtension).Assembly));

        services.AddTransient<IAppRecordsService, AppRecordsService>();
        services.AddTransient<IDbInfoRecordsService, DbInfoRecordsService>();
        services.AddTransient<ICheckSystemDbService, CheckSystemDbService>();
        
        return services;
    }
}