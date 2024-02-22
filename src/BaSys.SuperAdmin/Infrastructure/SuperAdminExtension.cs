using BaSys.SuperAdmin.Abstractions;
using BaSys.SuperAdmin.Controllers;
using BaSys.SuperAdmin.Data;
using BaSys.SuperAdmin.Services;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.EntityFrameworkCore;

namespace BaSys.SuperAdmin.Infrastructure;

public static class SuperAdminExtension
{
    public static IServiceCollection AddSuperAdmin(this IServiceCollection services, string systemDbConnectionString)
    {
        // add db context
        services.AddDbContext<SuperAdminDbContext>(options => options.UseSqlServer(systemDbConnectionString));
        
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