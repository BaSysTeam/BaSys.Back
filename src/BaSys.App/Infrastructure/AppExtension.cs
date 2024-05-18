using Microsoft.AspNetCore.Mvc.ApplicationParts;

namespace BaSys.App.Infrastructure
{
    public static class AppExtension
    {
        public static IServiceCollection AddApp(this IServiceCollection services)
        {
            // add controllers
            services.AddControllers()
                .PartManager
                .ApplicationParts
                .Add(new AssemblyPart(typeof(AppExtension).Assembly));

            return services;
        }
    }
}
