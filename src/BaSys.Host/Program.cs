using System.Text;
using BaSys.Common.Enums;
using BaSys.Common.Infrastructure;
using BaSys.Host.Abstractions;
using BaSys.Host.DAL;
using BaSys.Host.DAL.MsSqlContext;
using BaSys.Host.DAL.PgSqlContext;
using BaSys.Host.Data;
using BaSys.Host.Data.MsSqlContext;
using BaSys.Host.Data.PgSqlContext;
using BaSys.Host.Helpers;
using BaSys.Host.Infrastructure;
using BaSys.Host.Infrastructure.Interfaces;
using BaSys.Host.Infrastructure.JwtAuth;
using BaSys.Host.Services;
using BaSys.SuperAdmin.Abstractions;
using BaSys.SuperAdmin.Data.Identity;
using BaSys.SuperAdmin.Data.MsSqlContext;
using BaSys.SuperAdmin.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace BaSys.Host
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddScoped<IdentityDbContext>(sp =>
            {
                // var httpContextAccessor = sp.GetRequiredService<IHttpContextAccessor>();
                // // if sa
                // if (httpContextAccessor.HttpContext == null ||
                //     httpContextAccessor.HttpContext?.Request.Path.Value?.StartsWith("/sa/Account/") == true)
                //     return sp.GetRequiredService<MsSqlSuperAdminDbContext>();
                // else
                    return sp.GetRequiredService<ApplicationDbContext>();
            });

            builder.Services.AddScoped<IdentityDbContext<SaDbUser, SaDbRole, string>>(sp =>
            {
                return sp.GetRequiredService<MsSqlSuperAdminDbContext>();
            });

            builder.Services.AddScoped<ApplicationDbContext>(sp =>
            {
                var item = ContextHelper.GetConnectionItem(sp);
                switch (item?.DbKind)
                {
                    case DbKinds.MsSql:
                        return sp.GetRequiredService<MsSqlDbContext>();
                    case DbKinds.PgSql:
                        return sp.GetRequiredService<PgSqlDbContext>();
                    default:
                        throw new NotImplementedException($"Not implemented DbContext for type {item?.DbKind.ToString()}");
                }
            });

            // Add sa module
            builder.Services.AddSuperAdmin(builder.Configuration.GetSection("InitAppSettings"));

            // Add mssql context
            builder.Services.AddDbContext<MsSqlDbContext>((sp, options) =>
            {
                var item = ContextHelper.GetConnectionItem(sp, DbKinds.MsSql);
                if (item != null)
                    options.UseSqlServer(item.ConnectionString);
            });
            // Add pgsql context
            builder.Services.AddDbContext<PgSqlDbContext>((sp, options) =>
            {
                var item = ContextHelper.GetConnectionItem(sp, DbKinds.PgSql);
                if (item != null)
                    options.UseNpgsql(item.ConnectionString);
            });

            // Add default identity
            builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
                {
                    options.SignIn.RequireConfirmedAccount = false;
                    options.Password.RequireDigit = false;
                    options.Password.RequiredLength = GlobalConstants.PasswordMinLength;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                })
                .AddEntityFrameworkStores<IdentityDbContext>();
            
            // Add sa identity
            builder.Services.AddIdentityCore<SaDbUser>(options =>
                {
                    options.SignIn.RequireConfirmedAccount = false;
                    options.Password.RequireDigit = false;
                    options.Password.RequiredLength = GlobalConstants.PasswordMinLength;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                })
                .AddRoles<SaDbRole>()
                .AddSignInManager()
                .AddEntityFrameworkStores<MsSqlSuperAdminDbContext>();

            builder.Services.AddDatabaseDeveloperPageExceptionFilter();
            builder.Services.AddRazorPages();

            builder.Services.AddCors();

            builder.Services.AddAuthentication()
                .AddCookie()
                .AddJwtBearer(
                    opt =>
                    {
                        opt.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                                builder.Configuration["Jwt:TokenKey"] ??
                                throw new ApplicationException("Jwt:TokenKey is not set in the config!"))),
                            ValidateAudience = false,
                            ValidateIssuer = false,
                            RequireExpirationTime = true
                        };
                    });

            builder.Services.AddTransient<IJwtAuthService, JwtAuthService>();

            builder.Services.AddSingleton<IDataSourceProvider, DataSourceProvider>();
            // ToDo: delete this?
            // builder.Services.AddTransient<IContextFactory, ContextFactory>();
            builder.Services.AddTransient<IMainDbCheckService, MainDbCheckService>();

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

            app.UseCors(builder => builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.MapControllers();

            app.UseRouting();

            app.UseAuthorization();

            app.MapRazorPages();

            using var serviceScope = app.Services.CreateScope();
            var systemDbService = serviceScope.ServiceProvider.GetRequiredService<ICheckSystemDbService>();
            systemDbService.CheckAdminRolesEvent += async (initAppSettings) =>
            {
                using var serviceScopeInner = app.Services.CreateScope();
                var mainDbCheckService = serviceScopeInner.ServiceProvider.GetRequiredService<IMainDbCheckService>();
                await mainDbCheckService.Check(initAppSettings);
            };
            await systemDbService.CheckDbs();

            app.Run();
        }
    }
}