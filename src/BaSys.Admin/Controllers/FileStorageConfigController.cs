using BaSys.Admin.Abstractions;
using BaSys.Admin.DTO;
using BaSys.Common.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BaSys.Admin.Controllers;

[Route("api/admin/v1/[controller]")]
[ApiController]
// [Authorize(Roles = ApplicationRole.Administrator)]
public class FileStorageConfigController : ControllerBase
{
    private readonly IFileStorageConfigService _fileStorageConfigService;
    
    public FileStorageConfigController(IFileStorageConfigService fileStorageConfigService)
    {
        _fileStorageConfigService = fileStorageConfigService;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetFileStorageConfig()
    {
        var result = await _fileStorageConfigService.GetFileStorageConfigAsync();
        return Ok(result);
    }

    [HttpGet("GetStorageKinds")]
    public IActionResult GetStorageKinds()
    {
        var result = _fileStorageConfigService.GetStorageKinds();
        return Ok(result);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateFileStorageConfig(FileStorageConfigDto config)
    {
        var result = await _fileStorageConfigService.UpdateFileStorageConfigAsync(config);
        return Ok(result);
    }
}