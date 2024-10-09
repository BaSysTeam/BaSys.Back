using BaSys.Common.Infrastructure;
using BaSys.Core.Abstractions;
using BaSys.Metadata.Models.MenuModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BaSys.Constructor.Controllers
{
    [Route("api/constructor/v1/[controller]")]
    [ApiController]
    [Authorize(Roles = ApplicationRole.Designer)]
    public class MetaObjectsMenusController : ControllerBase
    {
        private readonly IMetaObjectMenusService _menuService;

        public MetaObjectsMenusController(IMetaObjectMenusService menuService)
        {
            _menuService = menuService;
        }

        [HttpGet()]
        public async Task<IActionResult> GetKindList()
        {
            var metaObjectSettings = await _menuService.GetKindListAsync();

            return Ok(metaObjectSettings);
        }

        [HttpGet("{name}")]
        public async Task<IActionResult> GetMetaObject(string name)
        {
            var metaObjectSettings = await _menuService.GetSettingsItemAsync(name);

            return Ok(metaObjectSettings);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMetaObject(MetaObjectMenuSettings settings)
        {
            var result = await _menuService.CreateAsync(settings);

            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateMetaObject(MetaObjectMenuSettings settings)
        {
            var result = await _menuService.UpdateSettingsItemAsync(settings);
            return Ok(result);
        }

        [HttpDelete("{name}")]
        public async Task<IActionResult> DeleteObject(string name)
        {
            var result = await _menuService.DeleteAsync(name);
            return Ok(result);
        }
    }
}
