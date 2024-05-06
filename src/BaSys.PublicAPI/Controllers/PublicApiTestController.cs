using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace BaSys.PublicAPI.Controllers;

[Route("api/public/v1/[controller]")]
[ApiController]
public class PublicApiTestController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Test()
    {
        return Ok("ok");
    }
}