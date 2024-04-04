using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace BaSys.Host.Controllers;
[Route("api/v1/[controller]")]
[ApiController]
public class TestController : ControllerBase
{
    private readonly Serilog.ILogger _logger;
    public TestController()
    {
        _logger = Log.Logger;
    }

    [HttpGet]
    public IActionResult Test()
    {
        _logger.Information("foo bar test");
        return Ok("foo");
    }
}