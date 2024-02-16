using BaSys.SuperAdmin.Data;
using BaSys.SuperAdmin.Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BaSys.SuperAdmin.Infrastructure;

public static class SuperAdminExtension
{
    public static IServiceCollection AddSuperAdmin(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<SuperAdminDbContext>(options =>
            options.UseSqlServer(connectionString));
        
        services.AddIdentityCore<SuperAdminIdentityUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
            })
            .AddEntityFrameworkStores<SuperAdminDbContext>();
        
        
        return services;
    }
}