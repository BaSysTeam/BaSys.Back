﻿using BaSys.Common.Infrastructure;
using BaSys.Host.Abstractions;
using BaSys.Host.DTO;
using Microsoft.AspNetCore.Mvc;

namespace BaSys.Host.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class MigrationController : ControllerBase
{
    private readonly IMigrationService _migrationService;
    public MigrationController(IMigrationService migrationService)
    {
        _migrationService = migrationService;
    }
    
    /// <summary>
    /// Get all exists migrations in system
    /// </summary>
    /// <returns></returns>
    [HttpGet("GetMigrations")]
    public IActionResult GetMigrations()
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
    /// Get applied migrations by DbName
    /// </summary>
    /// <returns></returns>
    [HttpGet("GetAppliedMigrations")]
    public async Task<IActionResult> GetAppliedMigrations([FromBody]string dbName)
    {
        var result = new ResultWrapper<IEnumerable<MigrationDto>>();

        try
        {
            var migrations = await _migrationService.GetAppliedMigrations(dbName);
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
    [HttpPost("MigrationUp")]
    public IActionResult MigrationUp([FromBody]Guid migrationUid)
    {
        throw new NotImplementedException();
    }
    
    /// <summary>
    /// Rollback migration with migrationUid
    /// </summary>
    /// <param name="migrationUid"></param>
    /// <returns></returns>
    [HttpPost("MigrationDown")]
    public IActionResult MigrationDown([FromBody]Guid migrationUid)
    {
        throw new NotImplementedException();
    }
}