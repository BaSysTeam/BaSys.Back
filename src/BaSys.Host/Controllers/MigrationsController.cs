using BaSys.Common.Infrastructure;
using BaSys.Host.Abstractions;
using BaSys.Host.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BaSys.Host.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
[Authorize(Roles = ApplicationRole.Administrator)]
public class MigrationsController : ControllerBase
{
    private readonly IMigrationService _migrationService;
    public MigrationsController(IMigrationService migrationService)
    {
        _migrationService = migrationService;
    }
    
    /// <summary>
    /// Get migrations with statuses
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var result = new ResultWrapper<IEnumerable<MigrationDto>>();

        try
        {
            var migrations = await _migrationService.GetMigrationList();
            if (migrations != null)
                result.Success(migrations);
            else
                result.Error(-1, "Error migration search");
        }
        catch (Exception e)
        {
            result.Error(-1, $"Error: {e}");
        }
        
        return Ok(result);
    }
    
    /// <summary>
    /// Apply migrations to migration with migrationUid
    /// </summary>
    /// <param name="migrationUid"></param>
    /// <returns></returns>
    [HttpPost("Up")]
    public async Task<IActionResult> Up([FromBody]Guid migrationUid)
    {
        var result = new ResultWrapper<bool>();

        try
        {
            var state = await _migrationService.MigrationUp(migrationUid);
            if (state)
                result.Success(state);
            else
                result.Error(-1, "MigrationUp false");
        }
        catch (Exception e)
        {
            result.Error(-1, $"MigrationUp false: {e.Message}");
        }
        
        return Ok(result);
    }
    
    /// <summary>
    /// Rollback last migration
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    [HttpPost("Down")]
    public async Task<IActionResult> Down()
    {
        var result = new ResultWrapper<bool>();
        var state = await _migrationService.MigrationDown();
        if (state)
            result.Success(state);
        else
            result.Error(-1, "MigrationDown false");
        
        return Ok(result);
    }
    
    /// <summary>
    /// Stop migration
    /// </summary>
    /// <returns></returns>
    [HttpGet("StopMigration")]
    public IActionResult StopMigration()
    {
        var result = new ResultWrapper<bool>();
        result.Success(_migrationService.StopMigration());
        
        return Ok(result);
    }

    /// <summary>
    /// Check is migrations running
    /// </summary>
    /// <returns></returns>
    [HttpGet("IsMigrationRun")]
    public IActionResult IsMigrationRun()
    {
        var result = new ResultWrapper<bool>();
        result.Success(_migrationService.GetMigrationStatus());
        
        return Ok(result);
    }
}