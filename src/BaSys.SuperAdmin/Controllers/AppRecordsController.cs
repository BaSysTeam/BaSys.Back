using BaSys.SuperAdmin.Abstractions;
using BaSys.SuperAdmin.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BaSys.SuperAdmin.Controllers;

[Route("api/[controller]")]
[ApiController]
#if !DEBUG
[Authorize]
#endif
public class AppRecordsController : ControllerBase
{
    private readonly IAppRecordsService _appRecordsService;
    
    public AppRecordsController(IAppRecordsService appRecordsService)
    {
        _appRecordsService = appRecordsService;
    }
    
    [HttpGet("GetAppRecords")]
    public async Task<IEnumerable<AppRecord>> GetAppRecords()
    {
        return await _appRecordsService.GetAppRecords();
    }
    
    [HttpPost("AddAppRecord")]
    public async Task<AppRecord> AddAppRecord([FromBody]AppRecord appRecord)
    {
        return await _appRecordsService.AddAppRecord(appRecord);
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