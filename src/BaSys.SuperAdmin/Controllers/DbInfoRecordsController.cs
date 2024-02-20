using BaSys.SuperAdmin.Abstractions;
using BaSys.SuperAdmin.Data.Models;
using Microsoft.AspNetCore.Mvc;

namespace BaSys.SuperAdmin.Controllers;

[Route("api/[controller]")]
public class DbInfoRecordsController : ControllerBase
{
    private readonly IDbInfoRecordsService _dbInfoRecordsService;
    
    public DbInfoRecordsController(IDbInfoRecordsService dbInfoRecordsService)
    {
        _dbInfoRecordsService = dbInfoRecordsService;
    }
    
    [HttpGet("GetDbInfoRecordsByAppId")]
    public async Task<IEnumerable<DbInfoRecord>> GetDbInfoRecordsByAppId([FromBody]string appId)
    {
        return await _dbInfoRecordsService.GetDbInfoRecordsByAppId(appId);
    }
    
    [HttpPost("AddDbInfoRecord")]
    public async Task<DbInfoRecord> AddDbInfoRecord([FromBody]DbInfoRecord appRecord)
    {
        return await _dbInfoRecordsService.AddDbInfoRecord(appRecord);
    }
    
    [HttpPost("EditDbInfoRecord")]
    public async Task<DbInfoRecord> EditDbInfoRecord([FromBody]DbInfoRecord appRecord)
    {
        return await _dbInfoRecordsService.EditDbInfoRecord(appRecord);
    }
    
    [HttpPost("DeleteDbInfoRecord")]
    public async Task<bool> DeleteDbInfoRecord([FromBody]int dbInfoRecordId)
    {
        return await _dbInfoRecordsService.DeleteDbInfoRecord(dbInfoRecordId);
    }
}