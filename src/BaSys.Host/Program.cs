using BaSys.Host.Data;
using BaSys.Host.Data.MsSqlContext;
using BaSys.Host.Data.PgSqlContext;
using BaSys.Host.Infrastructure;
using BaSys.Host.Providers;
using BaSys.SuperAdmin.Data;
using BaSys.SuperAdmin.Infrastructure;
using BaSys.SuperAdmin.Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BaSys.Host
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddScoped<ApplicationDbContext>(sp =>
            {
                var httpContextAccessor = sp.GetRequiredService<IHttpContextAccessor>();
                var userId = httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault()?.Value;

                ConnectionItem? item = null;
                if (!string.IsNullOrEmpty(userId))
                {
                    item = sp.GetRequiredService<IDataSourceProvider>().GetCurrentConnectionItemByUser(userId);
                }
                else if (httpContextAccessor.HttpContext?.Request.ContentType != null && 
                         httpContextAccessor.HttpContext?.Request.Form?.TryGetValue("Input.DbName", out var dbId) == true)
                {
                    item = sp.GetRequiredService<IDataSourceProvider>().GetConnectionItemByDbId(dbId);
                }
                else
                {
                    item = sp.GetRequiredService<IDataSourceProvider>().GetCurrentConnectionItemByUser(null);
                }
                
                switch (item.DbKind)
                {
                    case DbKinds.MsSql:
                        return sp.GetRequiredService<MsSqlDbContext>();
                    case DbKinds.PgSql:
                        return sp.GetRequiredService<PgSqlDbContext>();
                    default:
                        throw new NotImplementedException();
                }   
            });
            
            builder.Services.AddDbContext<MsSqlDbContext>((sp, options) =>
            {
                var httpContextAccessor = sp.GetRequiredService<IHttpContextAccessor>();
                var userId = httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault()?.Value;

                ConnectionItem? item = null;
                if (!string.IsNullOrEmpty(userId))
                {
                    item = sp.GetRequiredService<IDataSourceProvider>().GetCurrentConnectionItemByUser(userId);
                }
                else if (httpContextAccessor.HttpContext?.Request.ContentType != null && 
                         httpContextAccessor.HttpContext?.Request.Form?.TryGetValue("Input.DbName", out var dbId) == true)
                {
                    item = sp.GetRequiredService<IDataSourceProvider>().GetConnectionItemByDbId(dbId);
                }
                else
                {
                    item = sp.GetRequiredService<IDataSourceProvider>().GetCurrentConnectionItemByUser(null);
                }
            
                options.UseSqlServer(item.ConnectionString);
            });
            builder.Services.AddDbContext<PgSqlDbContext>((sp, options) =>
            {
                var httpContextAccessor = sp.GetRequiredService<IHttpContextAccessor>();
                var userId = httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault()?.Value;

                ConnectionItem? item = null;
                if (!string.IsNullOrEmpty(userId))
                {
                    item = sp.GetRequiredService<IDataSourceProvider>().GetCurrentConnectionItemByUser(userId);
                }
                else if (httpContextAccessor.HttpContext?.Request.ContentType != null && 
                         httpContextAccessor.HttpContext?.Request.Form?.TryGetValue("Input.DbName", out var dbId) == true)
                {
                    item = sp.GetRequiredService<IDataSourceProvider>().GetConnectionItemByDbId(dbId);
                }
                else
                {
                    item = sp.GetRequiredService<IDataSourceProvider>()
                        .GetConnectionItems()
                        .FirstOrDefault(x => x.DbKind == DbKinds.PgSql);
                }
            
                options.UseNpgsql(item.ConnectionString);
            });
            
            builder.Services.AddDefaultIdentity<IdentityUser>(options =>
                {
                    options.SignIn.RequireConfirmedAccount = true;
                    options.Password.RequireDigit = false;
                    options.Password.RequiredLength = 5;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                })
                .AddEntityFrameworkStores<ApplicationDbContext>();
            
            // Add sa context
            // builder.Services.AddSuperAdmin(builder.Configuration.GetConnectionString("SystemDbConnection")!);
            
            
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();
            builder.Services.AddRazorPages();
            
            builder.Services.AddSingleton<IDataSourceProvider, DataSourceProvider>();
            builder.Services.AddTransient<IContextFactory, ContextFactory>();
            

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.MapControllers();

            app.UseRouting();

            app.UseAuthorization();

            app.MapRazorPages();

            app.Run();
        }
    }
}
