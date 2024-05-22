using System.Data;
using System.Reflection;
using System.Text;
using BaSys.Admin.Infrastructure;
using BaSys.Common;
using BaSys.Common.Enums;
using BaSys.Common.Infrastructure;
using BaSys.Constructor.Infrastructure;
using BaSys.FileStorage.Infrastructure;
using BaSys.Host.Abstractions;
using BaSys.Host.DAL;
using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.DataProviders;
using BaSys.Host.DAL.MsSqlContext;
using BaSys.Host.DAL.PgSqlContext;
using BaSys.Host.Helpers;
using BaSys.Host.Identity;
using BaSys.Host.Identity.Models;
using BaSys.Host.Infrastructure;
using BaSys.Host.Infrastructure.Abstractions;
using BaSys.Host.Infrastructure.JwtAuth;
using BaSys.Host.Infrastructure.Providers;
using BaSys.Host.Middlewares;
using BaSys.Host.Services;
using BaSys.Logging.Abstractions.Abstractions;
using BaSys.Logging.Infrastructure;
using BaSys.PublicAPI.Infrastructure;
using BaSys.SuperAdmin.Abstractions;
using BaSys.SuperAdmin.DAL;
using BaSys.SuperAdmin.DAL.Abstractions;
using BaSys.SuperAdmin.DAL.Models;
using BaSys.SuperAdmin.Data.Identity;
using BaSys.SuperAdmin.Infrastructure;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Sinks.MSSqlServer;
using EnvironmentName = Microsoft.AspNetCore.Hosting.EnvironmentName;
using ILogger = Microsoft.Extensions.Logging.ILogger;

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

            // Add host version service
            builder.Services.AddSingleton<IHostVersionService>(provider =>
            {
                var version = Assembly.GetAssembly(typeof(Program))?.GetName()?.Version?.ToString() ?? string.Empty;
                return new HostVersionService(version);
            });

            // Add sa module
            builder.Services.AddSuperAdmin();

            // Add admin module
            builder.Services.AddAdmin();

            // Add constructor module
            builder.Services.AddConstructor();

            // Add logging module
            builder.Services.AddLog();
            
            // Add public api module
            builder.Services.AddPublicApi();

            // Add file storage
            builder.Services.AddFileStorage();

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
            builder.Services.AddTransient<IFileService, FileService>();

            builder.Services.AddSingleton<IDataSourceProvider, DataSourceProvider>();
            builder.Services.AddTransient<IMainDbCheckService, MainDbCheckService>();
            builder.Services.AddTransient<IWorkDbService, WorkDbService>();
            builder.Services.AddTransient<IHttpRequestContextService, HttpRequestContextService>();
            builder.Services.AddTransient<IUserSettingsService, UserSettingsService>();
            builder.Services.AddTransient<IMigrationService, MigrationService>();
            builder.Services.AddTransient<ILoggerService>(sp =>
            {
                var loggerFactory = sp.GetRequiredService<IBaSysLoggerFactory>();
                var logger = loggerFactory.GetLogger().GetAwaiter().GetResult();
                return logger;
            });

            // Factory to create DB connection by connection string and db kind.
            builder.Services.AddSingleton<IBaSysConnectionFactory, BaSysConnectionFactory>();
            builder.Services.AddTransient<IMainConnectionFactory, MainConnectionFactory>();

            // Factory to create TableManagers.
            builder.Services.AddTransient<ITableManagerFactory, TableManagerFactory>();

            // Factory to create system objects DataProviders.
            builder.Services.AddTransient<ISystemObjectProviderFactory, SystemObjectProviderFactory>();

            // Service to create system tables and fill constants when DB created.
            builder.Services.AddTransient<IDbInitService, DbInitService>();

            builder.Services.AddSingleton<MigrationRunnerService>();

            builder.Services.AddSwaggerGen(options => IncludeXmlCommentsHelper.IncludeXmlComments(options));

            Log.Logger = new LoggerConfiguration()
                .WriteTo
                //.Console()
                .MSSqlServer(
                    connectionString: "Data Source=OCEANSHIVERBOOK\\SQLEXPRESS;Initial Catalog=__Serilog;Persist Security Info=True;User ID=sa;Password=QAZwsx!@#;TrustServerCertificate=True;",
                    sinkOptions: new MSSqlServerSinkOptions { TableName = "LogEvents" })
                .CreateLogger();

            builder.Host.UseSerilog();
            builder.Logging.AddSerilog();

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

#if DEBUG
            app.UseMiddleware<CustomAuthorizationMiddleware>();
#endif

            app.MapRazorPages();

            using var serviceScope = app.Services.CreateScope();
            var systemDbService = serviceScope.ServiceProvider.GetRequiredService<ICheckSystemDbService>();
            systemDbService.CheckAdminRolesEvent += async (initAppSettings) =>
            {
                using var serviceScopeInner = app.Services.CreateScope();
                var mainDbCheckService = serviceScopeInner.ServiceProvider.GetRequiredService<IMainDbCheckService>();
                // Initialization by EF Context. Create users, roles etc.
                await mainDbCheckService.Check(initAppSettings);

                // Initialization by Dapper. Create system tables and fill necessary data when DB created.
                var connectionFactory = serviceScopeInner.ServiceProvider.GetRequiredService<IBaSysConnectionFactory>();
                var dbInitService = serviceScopeInner.ServiceProvider.GetRequiredService<IDbInitService>();

                var dbKind = initAppSettings.MainDb!.DbKind ?? DbKinds.PgSql;
                using (var connection = connectionFactory.CreateConnection(initAppSettings.MainDb.ConnectionString, dbKind))
                {
                    dbInitService.SetUp(connection);
                    await dbInitService.ExecuteAsync();
                }
            };
            await systemDbService.CheckDbs();

            var dbInfoRecordsProvider = serviceScope.ServiceProvider.GetRequiredService<IDbInfoRecordsProvider>();
            await dbInfoRecordsProvider.Update();

            app.Run();
        }
    }
}