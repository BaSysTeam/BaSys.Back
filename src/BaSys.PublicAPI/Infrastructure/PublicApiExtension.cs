using BaSys.PublicAPI.Controllers;
using Microsoft.AspNetCore.Mvc.ApplicationParts;

namespace BaSys.PublicAPI.Infrastructure;

public static class PublicApiExtension
{
    public static IServiceCollection AddPublicApi(this IServiceCollection services)
    {
        // add controllers
        services.AddControllers()
            .PartManager
            .ApplicationParts
            .Add(new AssemblyPart(typeof(PublicApiExtension).Assembly));
        
        return services;
    }
}