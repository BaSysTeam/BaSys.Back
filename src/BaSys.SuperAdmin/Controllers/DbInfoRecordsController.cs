using BaSys.Common.Infrastructure;
using BaSys.SuperAdmin.Abstractions;
using BaSys.SuperAdmin.Data.Models;
using Microsoft.AspNetCore.Mvc;

namespace BaSys.SuperAdmin.Controllers;

/// <summary>
/// This controller handles CRUD (Create, Read, Update, Delete) operations for DbInfoRecords.
/// </summary>
[Route("api/sa/v1/[controller]")]
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

    /// <summary>
    /// Get all records.
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetDbInfoRecords()
    {
        var result = new ResultWrapper<IEnumerable<DbInfoRecord>>();

        try
        {
            var collection = await _dbInfoRecordsService.GetDbInfoRecords();
            result.Success(collection);
        }
        catch (Exception ex)
        {
            result.Error(-1, $"Error retrieving records.", ex.Message);
            return Ok(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Retrieve record by Id.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetDbInfoRecord(int id)
    {
        var result = new ResultWrapper<DbInfoRecord>();

        try
        {
            var collection = await _dbInfoRecordsService.GetDbInfoRecord(id);
            if (collection == null)
                throw new Exception("collection is null");
            
            result.Success(collection);
        }
        catch (Exception ex)
        {
            result.Error(-1, $"Error retrieving records by id: {id}.", ex.Message);
            return Ok(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Create new record.
    /// </summary>
    /// <param name="appRecord"></param>
    /// <returns></returns>
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
            result.Error(-2, $"Cannot add record.", ex.Message);
        }

        return Ok(result);
    }

    /// <summary>
    /// Update record.
    /// </summary>
    /// <param name="appRecord"></param>
    /// <returns></returns>
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
            result.Error(-3, $"Cannot update record.", ex.Message);
        }

        return Ok(result);
    }

    /// <summary>
    /// Remove record by Id.
    /// </summary>
    /// <param name="id">Id of record</param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDbInfoRecord(int id)
    {
        var result = new ResultWrapper<int>();

        try
        {
            var deletedCount = await _dbInfoRecordsService.DeleteDbInfoRecord(id);
            result.Success(deletedCount); 
        }
        catch (Exception ex)
        {
            result.Error(-4, $"Cannot delete record by id: {id}.", ex.Message);
        }

        return Ok(result);
    }
}