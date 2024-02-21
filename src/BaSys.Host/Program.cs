using System.Text;
using BaSys.Common.Enums;
using BaSys.Common.Infrastructure.JwtAuth;
using BaSys.Host.Data;
using BaSys.Host.Data.MsSqlContext;
using BaSys.Host.Data.PgSqlContext;
using BaSys.Host.Helpers;
using BaSys.Host.Infrastructure;
using BaSys.Host.Providers;
using BaSys.SuperAdmin.Controllers;
using BaSys.SuperAdmin.Data;
using BaSys.SuperAdmin.Infrastructure;
using BaSys.SuperAdmin.Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace BaSys.Host
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddScoped<IdentityDbContext>(sp =>
            {
                var httpContextAccessor = sp.GetRequiredService<IHttpContextAccessor>();
                // if sa
                if (httpContextAccessor.HttpContext?.Request?.Method == "POST" &&
                    httpContextAccessor.HttpContext?.Request?.Path.Value.StartsWith("/sa/Account/") == true)
                    return sp.GetRequiredService<SuperAdminDbContext>();
                else
                    return sp.GetRequiredService<ApplicationDbContext>();
            });
            
            builder.Services.AddScoped<ApplicationDbContext>(sp =>
            {
                var item = ContextHelper.GetConnectionItem(sp);
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
            
            // Add sa context
            builder.Services.AddSuperAdmin(builder.Configuration.GetConnectionString("SystemDbConnection")!);
            // Add mssql context
            builder.Services.AddDbContext<MsSqlDbContext>((sp, options) =>
            {
                var item = ContextHelper.GetConnectionItem(sp, DbKinds.MsSql);
                options.UseSqlServer(item.ConnectionString);
            });
            // Add pgsql context
            builder.Services.AddDbContext<PgSqlDbContext>((sp, options) =>
            {
                var item = ContextHelper.GetConnectionItem(sp, DbKinds.PgSql);
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
                .AddEntityFrameworkStores<IdentityDbContext>();
            
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();
            builder.Services.AddRazorPages();
            
            builder.Services.AddAuthentication()
                .AddCookie()
                .AddJwtBearer(
                    opt =>
                    {
                        opt.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:TokenKey"])),
                            ValidateAudience = false,
                            ValidateIssuer = false,
                            RequireExpirationTime = true
                        };
                    });
            
            builder.Services.AddTransient<IJwtAuthService, JwtAuthService>();
            
            builder.Services.AddSingleton<IDataSourceProvider, DataSourceProvider>();
            builder.Services.AddTransient<IContextFactory, ContextFactory>();
            
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
                
                app.UseSwagger();
                app.UseSwaggerUI();
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
