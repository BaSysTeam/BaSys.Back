﻿using BaSys.Common.Enums;
using BaSys.SuperAdmin.Abstractions;
using BaSys.SuperAdmin.DAL;
using BaSys.SuperAdmin.DAL.Abstractions;
using BaSys.SuperAdmin.DAL.MsSqlContext;
using BaSys.SuperAdmin.DAL.PgSqlContext;
using BaSys.SuperAdmin.DAL.Providers;
using BaSys.SuperAdmin.Infrastructure.Models;
using BaSys.SuperAdmin.Services;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.EntityFrameworkCore;

namespace BaSys.SuperAdmin.Infrastructure;

public static class SuperAdminExtension
{
    public static IServiceCollection AddSuperAdmin(this IServiceCollection services)
    {
        // var initAppSettings = configurationSection.Get<InitAppSettings>();
        // if (initAppSettings == null)
        //     throw new ApplicationException("InitAppSettings is not set in the config!");
        // if (string.IsNullOrEmpty(initAppSettings.Sa?.ConnectionString))
        //     throw new ApplicationException("InitAppSettings:Sa:ConnectionString is not set in the config!");

        // var connectionString = initAppSettings.Sa.ConnectionString;

        // add db context
        services.AddScoped<SuperAdminDbContext>(sp =>
        {
            var configuration = sp.GetRequiredService<IConfiguration>();
            var initAppSettings = configuration.GetSection("InitAppSettings").Get<InitAppSettings>();
            switch (initAppSettings?.Sa?.DbKind)
            {
                case DbKinds.MsSql:
                    return sp.GetRequiredService<MsSqlSuperAdminDbContext>();
                case DbKinds.PgSql:
                    return sp.GetRequiredService<PgSqlSuperAdminDbContext>();
                default:
                    throw new NotImplementedException(
                        $"Not implemented SuperAdmin DbContext for type {initAppSettings?.Sa?.DbKind.ToString()}");
            }
        });
        services.AddDbContext<MsSqlSuperAdminDbContext>((sp, options) =>
        {
            var configuration = sp.GetRequiredService<IConfiguration>();
            var initAppSettings = configuration.GetSection("InitAppSettings").Get<InitAppSettings>();
            var connectionString = initAppSettings?.Sa?.ConnectionString;
            options.UseSqlServer(connectionString);
        });
        services.AddDbContext<PgSqlSuperAdminDbContext>((sp, options) =>
        {
            var configuration = sp.GetRequiredService<IConfiguration>();
            var initAppSettings = configuration.GetSection("InitAppSettings").Get<InitAppSettings>();
            var connectionString = initAppSettings?.Sa?.ConnectionString;
            options.UseNpgsql(connectionString);
        });

        // add controllers
        services.AddControllers()
            .PartManager
            .ApplicationParts
            .Add(new AssemblyPart(typeof(SuperAdminExtension).Assembly));

        services.AddSingleton<IDbInfoRecordsProvider, DbInfoRecordsProvider>();
            
        services.AddTransient<IAppRecordsService, AppRecordsService>();
        services.AddTransient<IDbInfoRecordsService, DbInfoRecordsService>();
        services.AddTransient<ICheckSystemDbService, CheckSystemDbService>();
        services.AddTransient<ICheckDbExistsService, CheckDbExistsService>();

        return services;
    }
}