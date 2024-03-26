using BaSys.Common.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BaSys.Constructor.Controllers
{
    [Route("api/constructor/v1/[controller]")]
    [ApiController]
    public class ConstructorTestController : ControllerBase
    {
        [HttpGet("Ping")]
        public IActionResult Ping()
        {
            var result = new ResultWrapper<DateTime>();
            result.Success(DateTime.Now);

            return Ok(result);  
        }
    }
}
