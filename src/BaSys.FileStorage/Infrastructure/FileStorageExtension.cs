using BaSys.FileStorage.Abstractions;
using BaSys.FileStorage.S3.Abstractions;
using BaSys.FileStorage.S3.Services;
using BaSys.FileStorage.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BaSys.FileStorage.Infrastructure;

public static class FileStorageExtension
{
    public static IServiceCollection AddFileStorage(this IServiceCollection services)
    {
        services.AddTransient<IFileStorageServiceFactory, FileStorageServiceFactory>();

        return services;
    }
}