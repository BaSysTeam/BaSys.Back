using BaSys.Common.Infrastructure;
using BaSys.SuperAdmin.Abstractions;
using BaSys.SuperAdmin.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BaSys.SuperAdmin.Controllers;

/// <summary>
/// This controller handles CRUD (Create, Read, Update, Delete) operations for app records.
/// </summary>
[Route("api/sa/v1/[controller]")]
[ApiController]
#if !DEBUG
[Authorize(TeamRole.SuperAdministrator)]
#endif
public class AppRecordsController : ControllerBase
{
    private readonly IAppRecordsService _appRecordsService;

    public AppRecordsController(IAppRecordsService appRecordsService)
    {
        _appRecordsService = appRecordsService;
    }

    // HTTP GET method to retrieve all app records. Asynchronous action that returns
    // an IActionResult. Wraps the result in a success wrapper and returns HTTP 200 with the data.
    [HttpGet]
    public async Task<IActionResult> GetAppRecords()
    {
        var collection = await _appRecordsService.GetAppRecords();
        var payload = new ResultWrapper<IEnumerable<AppRecord>>();
        payload.Success(collection);

        return Ok(payload);
    }

    // HTTP GET method to retrieve a single app record by its ID. The ID is specified in the route.
    [HttpGet("{id}")]
    public async Task<IActionResult> GetAppRecordById(string id)
    {
        var payload = new ResultWrapper<AppRecord>();
        try
        {
            var record = await _appRecordsService.GetAppRecord(id);
            if (record == null)
            {
                payload.Error(-1, $"Cannot find record by id: {id}");
                return Ok(payload);
            }
            payload.Success(record);
            return Ok(payload);
        }
        catch (Exception e)
        {
            // Wrap and return any exceptions encountered during the retrieval process.
            payload.Error(-1, $"Error retrieving record by id: {id}.", e.Message);
            return Ok(payload); 
        }
    }


    // HTTP POST method to add a new app record. Takes an AppRecord object from the request body.
    // If the operation is successful, it wraps the added record in a success wrapper.
    // If there's an exception, it wraps the error message and returns it.
    [HttpPost]
    public async Task<IActionResult> AddAppRecord([FromBody] AppRecord appRecord)
    {

        var payload = new ResultWrapper<AppRecord>();

        try
        {
            var record = await _appRecordsService.AddAppRecord(appRecord);
            payload.Success(record);
        }
        catch (Exception e)
        {
            payload.Error(-2, $"Cannot add item.", e.Message);
        }

        return Ok(payload);
    }

    // HTTP PUT method for updating an existing app record with a new value.
    // It attempts to update the record and wraps the result in a success wrapper.
    // In case of failure, wraps an error message instead.
    [HttpPut]
    public async Task<IActionResult> UpdateAppRecord([FromBody] AppRecord appRecord)
    {
        var payload = new ResultWrapper<AppRecord>();

        try
        {
            var record = await _appRecordsService.UpdateAppRecord(appRecord);
            payload.Success(record);
        }
        catch (Exception e)
        {
            payload.Error(-3, $"Cannot add item.", e.Message);
        }

        return Ok(payload);
    }

    /// <summary>
    /// HTTP DELETE method to remove an app record by its id. The id is passed as a query parameter.
    /// It attempts to delete the record and returns the count of deleted records wrapped in a success wrapper.
    /// In case of failure, it wraps an error message and returns it.
    /// </summary>
    /// <param name="id">Identifier of record</param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAppRecord(string id)
    {
        var payload = new ResultWrapper<int>();

        try
        {
            var deletedCount = await _appRecordsService.DeleteAppRecord(id);
            payload.Success(deletedCount);
        }
        catch (Exception e)
        {
            payload.Error(-4, $"Cannot delete item by id: {id}.", e.Message);
        }

        return Ok(payload);
    }


}