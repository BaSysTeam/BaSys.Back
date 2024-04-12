using BaSys.Common.Infrastructure;
using BaSys.Host.Abstractions;
using BaSys.Host.DTO;
using Microsoft.AspNetCore.Mvc;

namespace BaSys.Host.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class MigrationsController : ControllerBase
{
    private readonly IMigrationService _migrationService;
    public MigrationsController(IMigrationService migrationService)
    {
        _migrationService = migrationService;
    }
    
    /// <summary>
    /// Get all exists migrations in system
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public IActionResult Get()
    {
        var result = new ResultWrapper<IEnumerable<MigrationDto>>();

        try
        {
            var migrations = _migrationService.GetMigrations();
            if (migrations != null)
                result.Success(migrations.Select(x => new MigrationDto(x)));
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
    /// Get applied migrations
    /// </summary>
    /// <returns></returns>
    [HttpGet("GetAppliedMigrations")]
    public async Task<IActionResult> GetAppliedMigrations()
    {
        var result = new ResultWrapper<IEnumerable<MigrationDto>>();

        try
        {
            var migrations = await _migrationService.GetAppliedMigrations();
            if (migrations != null)
                result.Success(migrations.Select(x => new MigrationDto(x)));
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
    
    [HttpGet("StopMigration")]
    public IActionResult StopMigration()
    {
        var result = new ResultWrapper<bool>();
        result.Success(_migrationService.StopMigration());
        
        return Ok(result);
    }

    // [HttpGet("GetStatus")]
    // public IActionResult GetStatus([FromBody]Guid requestUid)
    // {
    //     var result = new ResultWrapper<MigrationStatusDto>();
    //     if (requestUid == Guid.Empty)
    //         result.Success(new MigrationStatusDto());
    //     
    //     return Ok(result);
    // }
}