using BaSys.Common.Infrastructure;
using BaSys.SuperAdmin.Abstractions;
using BaSys.SuperAdmin.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BaSys.SuperAdmin.Controllers;

[Route("api/v1/[controller]")]
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
    
    [HttpGet]
    public async Task<IActionResult> GetAppRecords()
    {
        var collection = await _appRecordsService.GetAppRecords();
        var result = ResultWrapper<IEnumerable<AppRecord>>.Success(collection);

        return Ok(result);
    }
    
    [HttpPost]
    public async Task<IActionResult> AddAppRecord([FromBody]AppRecord appRecord)
    {

        ResultWrapper<AppRecord> result;

        try
        {
           var record = await _appRecordsService.AddAppRecord(appRecord);
           result = ResultWrapper<AppRecord>.Success(record);
        }
        catch (Exception e)
        {
            result = ResultWrapper<AppRecord>.Error(-1, $"Cannot add item. Message: {e.Message}");
        }

        return Ok(result);
    }
    
    [HttpPost("DeleteAppRecord")]
    public async Task<bool> DeleteAppRecord([FromBody]string appRecordId)
    {
        return await _appRecordsService.DeleteAppRecord(appRecordId);
    }
    
    [HttpPost("EditAppRecord")]
    public async Task<AppRecord> EditAppRecord([FromBody]AppRecord appRecord)
    {
        return await _appRecordsService.EditAppRecord(appRecord);
    }
}