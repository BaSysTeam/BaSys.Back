namespace BaSys.FileStorage.Abstractions;

public interface IFileStorageServiceFactory
{
    Task<IFileStorageService?> GetServiceAsync();
}