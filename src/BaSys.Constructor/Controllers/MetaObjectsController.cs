using BaSys.Common.Infrastructure;
using BaSys.Constructor.Abstractions;
using BaSys.Core.Abstractions;
using BaSys.DTO.Metadata;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BaSys.Constructor.Controllers
{
    [Route("api/constructor/v1/[controller]")]
    [ApiController]
    [Authorize(Roles = ApplicationRole.Administrator)]
    public class MetaObjectsController : ControllerBase
    {
        private readonly IMetaObjectsService _metaObjectService;

        public MetaObjectsController(IMetaObjectsService metaObjectService)
        {
            _metaObjectService = metaObjectService; 
        }

        [HttpGet("{kind}/{name}")]
        public async Task<IActionResult> GetMetaObject(string kind, string name)
        {
            var metaObjectSettings = await _metaObjectService.GetSettingsItemAsync(kind, name);

            return Ok(metaObjectSettings);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateMetaObject(MetaObjectStorableSettingsDto settingsDto)
        {
            var result = await _metaObjectService.UpdateSettingsItemAsync(settingsDto);
            return Ok(result);
        }
    }
}
