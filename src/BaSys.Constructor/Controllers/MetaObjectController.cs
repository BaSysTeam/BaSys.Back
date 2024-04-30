using BaSys.Common.Infrastructure;
using BaSys.Constructor.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BaSys.Constructor.Controllers
{
    [Route("api/constructor/v1/[controller]")]
    [ApiController]
    [Authorize(Roles = ApplicationRole.Administrator)]
    public class MetaObjectController : ControllerBase
    {
        private readonly IMetaObjectService _metaObjectService;

        public MetaObjectController(IMetaObjectService metaObjectService)
        {
            _metaObjectService = metaObjectService; 
        }

        [HttpGet("{kind}/{name}")]
        public async Task<IActionResult> GetMetaObject(string kind, string name)
        {
            var metaObjectSettings = await _metaObjectService.GetSettingsItemAsync(kind, name);

            return Ok(metaObjectSettings);
        }
    }
}
