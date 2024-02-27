using BaSys.Common.Infrastructure;
using BaSys.SuperAdmin.Abstractions;
using BaSys.SuperAdmin.Data.Models;
using Microsoft.AspNetCore.Mvc;

namespace BaSys.SuperAdmin.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
#if !DEBUG
[Authorize(TeamRole.SuperAdministrator)]
#endif
public class DbInfoRecordsController : ControllerBase
{
    private readonly IDbInfoRecordsService _dbInfoRecordsService;

    public DbInfoRecordsController(IDbInfoRecordsService dbInfoRecordsService)
    {
        _dbInfoRecordsService = dbInfoRecordsService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetDbInfoRecordsByAppId(string id)
    {
        var result = new ResultWrapper<IEnumerable<DbInfoRecord>>();

        try
        {
            var collection = await _dbInfoRecordsService.GetDbInfoRecordsByAppId(id);
            result.Success(collection);
        }
        catch (Exception ex)
        {
            result.Error(-1, $"Error retrieving records by id: {id}. Message: {ex.Message}");
            return Ok(result);
        }

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> AddDbInfoRecord([FromBody] DbInfoRecord appRecord)
    {
        var result = new ResultWrapper<DbInfoRecord>();

        try
        {
            var record = await _dbInfoRecordsService.AddDbInfoRecord(appRecord);
            result.Success(record);
        }
        catch (Exception ex)
        {
            result.Error(-2, $"Cannot add record. Message: {ex.Message}");
        }

        return Ok(result);
    }

    [HttpPut]
    public async Task<IActionResult> EditDbInfoRecord([FromBody] DbInfoRecord appRecord)
    {
        var result = new ResultWrapper<DbInfoRecord>();

        try
        {
            var record = await _dbInfoRecordsService.EditDbInfoRecord(appRecord);
            result.Success(record);
        }
        catch (Exception ex)
        {
            result.Error(-3, $"Cannot update record. Message: {ex.Message}");
        }

        return Ok(result);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteDbInfoRecord([FromQuery] int id)
    {
        var result = new ResultWrapper<int>();

        try
        {
            var deletedCount = await _dbInfoRecordsService.DeleteDbInfoRecord(id);
            result.Success(deletedCount); 
        }
        catch (Exception ex)
        {
            result.Error(-4, $"Cannot delete record by id: {id}. Message: {ex.Message}");
        }

        return Ok(result);
    }
}