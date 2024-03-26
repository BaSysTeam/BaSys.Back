using BaSys.SuperAdmin.DAL.Abstractions;
using BaSys.SuperAdmin.DAL.Providers;
using Microsoft.AspNetCore.Mvc.ApplicationParts;

namespace BaSys.Constructor.Infrastructure
{
    public static class ConstructorExtension
    {
        public static IServiceCollection AddConstructor(this IServiceCollection services)
        {
            // add controllers
            services.AddControllers()
                .PartManager
                .ApplicationParts
                .Add(new AssemblyPart(typeof(ConstructorExtension).Assembly));


            return services;
        }
    }
}
