using BaSys.Common.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BaSys.Constructor.Controllers
{
    [Route("api/constructor/v1/[controller]")]
    [ApiController]
    public class ConstructorTestController : ControllerBase
    {
        private IConfiguration _configuration;

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
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var dbName = User.Claims.FirstOrDefault(c => c.Type == "DbName")?.Value;

            var result = new ResultWrapper<string>();
            result.Success(dbName);

         

            return Ok(result);
        }
    }
}
