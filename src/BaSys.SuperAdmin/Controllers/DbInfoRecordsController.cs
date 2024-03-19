using BaSys.Common.Infrastructure;
using BaSys.SuperAdmin.Abstractions;
using BaSys.SuperAdmin.DAL.Models;
using BaSys.SuperAdmin.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BaSys.SuperAdmin.Controllers;

/// <summary>
/// This controller handles CRUD (Create, Read, Update, Delete) operations for DbInfoRecords.
/// </summary>
[Route("api/sa/v1/[controller]")]
[ApiController]
#if !DEBUG
[Authorize(ApplicationRole.SuperAdministrator)]
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
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> AddDbInfoRecord([FromBody] DbInfoRecordDto dto)
    {
        if (dto == null)
            throw new ArgumentException("dto is null");
        
        var result = new ResultWrapper<DbInfoRecord>();
        var model = new DbInfoRecord
        {
            AppId = dto.AppId ?? string.Empty,
            Name = dto.Name ?? string.Empty,
            Title = dto.Title,
            DbKind = dto.DbKind,
            ConnectionString = dto.ConnectionString ?? string.Empty,
            Memo = dto.Memo
        };
        
        try
        {
            var record = await _dbInfoRecordsService.AddDbInfoRecord(model);
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
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpPut]
    public async Task<IActionResult> EditDbInfoRecord([FromBody] DbInfoRecordDto dto)
    {
        if (dto == null)
            throw new ArgumentException("dto is null");
        
        var result = new ResultWrapper<DbInfoRecord>();
        var model = new DbInfoRecord
        {
            Id = dto.Id,
            AppId = dto.AppId ?? string.Empty,
            Name = dto.Name ?? string.Empty,
            Title = dto.Title,
            DbKind = dto.DbKind,
            ConnectionString = dto.ConnectionString ?? string.Empty,
            Memo = dto.Memo
        };
        
        try
        {
            var record = await _dbInfoRecordsService.EditDbInfoRecord(model);
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
    
    //

    /// <summary>
    /// Switch activity record by setting inverse IsDeleted.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpPatch("{id}/switchactivity")]
    public async Task<IActionResult> SwitchActivityDbInfoRecord(int id)
    {
        var result = new ResultWrapper<DbInfoRecord>();

        try
        {
            var record = await _dbInfoRecordsService.SwitchActivityDbInfoRecord(id);
            result.Success(record);
        }
        catch (Exception ex)
        {
            result.Error(-3, $"Cannot switch activity record.", ex.Message);
        }

        return Ok(result);
    }

    [HttpPatch("{id}/initdb")]
    public async Task<IActionResult> InitDb(int id)
    {
        var result = new ResultWrapper<bool>();
        
        // try
        // {
        //     var state = await _dbInfoRecordsService.InitDb(id);
        //     result.Success(state);
        // }
        // catch (Exception ex)
        // {
        //     result.Error(-3, $"Cannot switch activity record.", ex.Message);
        // }
        
        return Ok(result);
    }
}