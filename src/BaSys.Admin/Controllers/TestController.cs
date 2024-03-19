using BaSys.Common.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BaSys.Admin.Controllers
{
    [Route("api/admin/v1/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        [HttpGet]
        public IActionResult Ping()
        {
            var result = new ResultWrapper<int>();
            result.Success(1, $"Done at {DateTime.Now}");

            return Ok(result);
        }
    }
}
