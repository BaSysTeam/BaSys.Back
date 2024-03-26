using BaSys.Common.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using BaSys.SuperAdmin.DAL.Providers;

namespace BaSys.Constructor.Controllers
{
    [Route("api/constructor/v1/[controller]")]
    [ApiController]
    public class ConstructorTestController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly DbInfoRecordsProvider _dbInfoRecordsProvider;

        //public ConstructorTestController(IConfiguration configuration, DbInfoRecordsProvider dbInfoRecordsProvider)
        //{
        //    _configuration = configuration;
        //    _dbInfoRecordsProvider = dbInfoRecordsProvider; 
        //}

        public ConstructorTestController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("Ping")]
        public IActionResult Ping()
        {
            var result = new ResultWrapper<DateTime>();
            result.Success(DateTime.Now);

            return Ok(result);  
        }

        [HttpPost("MetadataGroupTable")]
        public IActionResult CreateMetadataGroupTable()
        {
            //var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = new ResultWrapper<string>();
            var dbName = User.Claims.FirstOrDefault(c => c.Type == "DbName")?.Value;

            if(string.IsNullOrEmpty(dbName) )
            {
                result.Error(-1, $"Cannot get dbName.");
                return Ok(result);
            }

            // TODO: Get connection strings via service.
            //var dbInfoRecord = _dbInfoRecordsProvider.GetDbInfoRecordByDbName(dbName);

            //if ( dbInfoRecord == null )
            //{
            //    result.Error(-1, $"Cannot get DbInfoRecord.");
            //    return Ok(result);
            //}

            var mainDbConnectionString = _configuration.GetSection("InitAppSettings:MainDb:ConnectionString").Value;

            if (string.IsNullOrEmpty(mainDbConnectionString))
            {
                result.Error(-1, $"Cannot get connectionSring.");
                return Ok(result);
            }

            result.Success(dbName);

         

            return Ok(result);
        }
    }
}
