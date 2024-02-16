using Microsoft.AspNetCore.Mvc;

namespace BaSys.Host.Controllers;

[Route("api/[controller]")]
public class TestController : ControllerBase
{
    [HttpGet]
    public string GetTest()
    {
        return "Test!";
    }
}