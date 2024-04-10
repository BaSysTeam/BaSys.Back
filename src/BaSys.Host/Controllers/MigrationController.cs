using Microsoft.AspNetCore.Mvc;

namespace BaSys.Host.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class MigrationController : ControllerBase
{
    /// <summary>
    /// Get all exists migrations in system
    /// </summary>
    /// <returns></returns>
    [HttpGet("GetMigrations")]
    public IActionResult GetMigrations()
    {
        return Ok();
    }
    
    /// <summary>
    /// Get applied migrations
    /// </summary>
    /// <returns></returns>
    [HttpGet("GetAppliedMigrations")]
    public IActionResult GetAppliedMigrations([FromBody]string dbName)
    {
        return Ok();
    }
    
    [HttpPost("MigrationUp")]
    public IActionResult Test([FromBody]Guid migrationUid)
    {
        return Ok();
    }
}