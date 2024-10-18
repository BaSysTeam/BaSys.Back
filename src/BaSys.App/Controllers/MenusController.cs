using BaSys.App.Abstractions;
using BaSys.Common.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BaSys.App.Controllers
{
    [Route("api/app/v1/[controller]")]
    [ApiController]
    [Authorize(Roles = ApplicationRole.User)]
    public class MenusController : ControllerBase
    {
        private readonly IMenusService _menusService;

        public MenusController(IMenusService menusService)
        {
            _menusService = menusService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCollection() { 

            var result = await _menusService.GetCollectionAsync();

            return Ok(result);
        
        }
    }
}
