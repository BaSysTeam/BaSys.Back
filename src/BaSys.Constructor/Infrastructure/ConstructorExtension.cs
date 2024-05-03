using BaSys.Constructor.Abstractions;
using BaSys.Constructor.Services;
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

            services.AddTransient<IMetaObjectKindsService, MetaObjectKindsService>();
            services.AddTransient<IMetadataTreeNodesService, MetadataTreeNodesService>();
            services.AddTransient<IMetaObjectsService, MetaObjectsService>(); 

            return services;
        }
    }
}
