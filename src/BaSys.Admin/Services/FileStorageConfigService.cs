using System.Data;
using BaSys.Admin.Abstractions;
using BaSys.Admin.DTO;
using BaSys.Common.DTO;
using BaSys.Common.Enums;
using BaSys.Common.Infrastructure;
using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.DataProviders;
using MongoDB.Driver;

namespace BaSys.Admin.Services;

public class FileStorageConfigService : IFileStorageConfigService
{
    private readonly IDbConnection _connection;
    private readonly FileStorageConfigProvider _provider;

    public FileStorageConfigService(IMainConnectionFactory mainConnectionFactory,
        ISystemObjectProviderFactory providerFactory)
    {
        _connection = mainConnectionFactory.CreateConnection();
        providerFactory.SetUp(_connection);
        _provider = providerFactory.Create<FileStorageConfigProvider>();
    }

    public async Task<ResultWrapper<FileStorageConfigDto>> GetFileStorageConfigAsync()
    {
        var result = new ResultWrapper<FileStorageConfigDto>();

        var config = (await _provider.GetCollectionAsync(null)).FirstOrDefault();
        if (config == null)
            result.Success(new FileStorageConfigDto());
        else
            result.Success(new FileStorageConfigDto(config));

        return result;
    }

    public async Task<ResultWrapper<bool>> UpdateFileStorageConfigAsync(FileStorageConfigDto fileStorageConfig)
    {
        var result = new ResultWrapper<bool>();

        int count;
        var config = (await _provider.GetCollectionAsync(null)).FirstOrDefault();
        if (config == null)
        {
            await _provider.InsertAsync(fileStorageConfig.ToModel(), null);
            count = 1;
        }
        else
        {
            count = await _provider.UpdateAsync(fileStorageConfig.ToModel(), null);
        }

        if (count == 1)
            result.Success(true);
        else
            result.Error(-1, "Error save config");

        return result;
    }

    public ResultWrapper<List<EnumValuesDto>> GetStorageKinds()
    {
        var result = new ResultWrapper<List<EnumValuesDto>>();
        var languages = new List<EnumValuesDto>();
        foreach (var lang in (FileStorageKinds[])Enum.GetValues(typeof(FileStorageKinds)))
        {
            languages.Add(new EnumValuesDto
            {
                Id = (int)lang,
                Name = lang.ToString()
            });
        }

        result.Success(languages);
        return result;
    }
}