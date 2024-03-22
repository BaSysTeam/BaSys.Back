using System.Text;
using BaSys.Admin.Infrastructure;
using BaSys.Common.Enums;
using BaSys.Common.Infrastructure;
using BaSys.Host.Abstractions;
using BaSys.Host.DAL;
using BaSys.Host.DAL.MsSqlContext;
using BaSys.Host.DAL.PgSqlContext;
using BaSys.Host.Identity;
using BaSys.Host.Identity.Models;
using BaSys.Host.Infrastructure;
using BaSys.Host.Infrastructure.Interfaces;
using BaSys.Host.Infrastructure.JwtAuth;
using BaSys.Host.Services;
using BaSys.SuperAdmin.Abstractions;
using BaSys.SuperAdmin.DAL;
using BaSys.SuperAdmin.Data.Identity;
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

            builder.Services.AddScoped<IdentityDbContext<WorkDbUser, WorkDbRole, string>>(sp =>
            {
                return sp.GetRequiredService<ApplicationDbContext>();
            });

            builder.Services.AddScoped<IdentityDbContext<SaDbUser, SaDbRole, string>>(sp =>
            {
                return sp.GetRequiredService<SuperAdminDbContext>();
            });

            // Add WorkDb base context
            builder.Services.AddScoped<ApplicationDbContext>(sp =>
            {
                var dbKind = sp.GetRequiredService<IHttpRequestContextService>().GetConnectionKind();
                switch (dbKind)
                {
                    case DbKinds.MsSql:
                        return sp.GetRequiredService<MsSqlDbContext>();
                    case DbKinds.PgSql:
                        return sp.GetRequiredService<PgSqlDbContext>();
                    default:
                        throw new NotImplementedException($"Not implemented DbContext for type {dbKind.ToString()}");
                }
            });

            // Add sa module
            builder.Services.AddSuperAdmin(builder.Configuration.GetSection("InitAppSettings"));

            // Add admin module
            builder.Services.AddAdmin();

            // Add mssql context
            builder.Services.AddDbContext<MsSqlDbContext>((sp, options) =>
            {
                var item = sp.GetRequiredService<IHttpRequestContextService>().GetConnectionItem(DbKinds.MsSql);
                options.UseSqlServer(item?.ConnectionString ?? string.Empty);
            });
            // Add pgsql context
            builder.Services.AddDbContext<PgSqlDbContext>((sp, options) =>
            {
                var item = sp.GetRequiredService<IHttpRequestContextService>().GetConnectionItem(DbKinds.PgSql);
                options.UseNpgsql(item?.ConnectionString ?? string.Empty);
            });

            // Add default identity
            builder.Services.AddIdentity<WorkDbUser, WorkDbRole>(options =>
                {
                    options.SignIn.RequireConfirmedAccount = false;
                    options.Password.RequireDigit = false;
                    options.Password.RequiredLength = GlobalConstants.PasswordMinLength;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                })
                .AddEntityFrameworkStores<IdentityDbContext<WorkDbUser, WorkDbRole, string>>();

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
                .AddEntityFrameworkStores<SuperAdminDbContext>();

            builder.Services.AddDatabaseDeveloperPageExceptionFilter();
            builder.Services.AddRazorPages();

            builder.Services.AddCors();

            builder.Services.AddAuthentication(
                    options =>
                    {
                        // options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                        // options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                        // options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    })
                .AddCookie(options => { options.LoginPath = new PathString("/Identity/Account/Login"); })
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
            builder.Services.AddTransient<IMainDbCheckService, MainDbCheckService>();
            builder.Services.AddTransient<IWorkDbService, WorkDbService>();
            builder.Services.AddTransient<IHttpRequestContextService, HttpRequestContextService>();

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

            app.UseAuthentication(); // Ensure authentication is used
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