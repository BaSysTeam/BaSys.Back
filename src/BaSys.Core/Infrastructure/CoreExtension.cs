using BaSys.Core.Abstractions;
using BaSys.Core.Services;

namespace BaSys.Core.Infrastructure;

public static class CoreExtension
{
    public static IServiceCollection AddCore(this IServiceCollection services)
    {
        services.AddTransient<IMetaObjectKindsService, MetaObjectKindsService>();
        services.AddTransient<IMetaObjectsService, MetaObjectsService>();
        services.AddTransient<IMetaMenusService, MetaMenusService>();
        services.AddTransient<IDataTypesService, DataTypesService>();
        services.AddTransient<IMetadataReader, MetadataReader>();

        return services;
    }
}