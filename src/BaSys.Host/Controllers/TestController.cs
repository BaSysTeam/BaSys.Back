using BaSys.Host.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace BaSys.Host.Controllers;
[Route("api/v1/[controller]")]
[ApiController]
public class TestController : ControllerBase
{
    private readonly IMegaMenuService _megaMenuService;

    public TestController(IMegaMenuService megaMenuService)
    {
        _megaMenuService = megaMenuService;
    }

    [HttpGet("MegaMenuItems")]
    public IActionResult GetMegaMenuItems()
    {
        var items = _megaMenuService.GetItems();
        return Ok(items);
    }
}