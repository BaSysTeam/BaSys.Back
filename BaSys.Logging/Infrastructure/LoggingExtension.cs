using BaSys.Logging.Abstractions;
using BaSys.Logging.Abstractions.Abstractions;
using BaSys.Logging.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BaSys.Logging.Infrastructure;

public static class SuperAdminExtension
{
    public static IServiceCollection AddLog(this IServiceCollection services)
    {
        services.AddTransient<ILoggerFactory, LoggerFactory>();
        services.AddTransient<ILoggerConfigService, LoggerConfigService>();
        
        return services;
    }
}