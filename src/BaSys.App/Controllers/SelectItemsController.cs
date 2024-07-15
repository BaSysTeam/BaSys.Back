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
    public class SelectItemsController : ControllerBase
    {
        private readonly ISelectItemService _service;

        public SelectItemsController(ISelectItemService service)
        {
            _service = service;
        }

        [HttpGet("{dataTypeUid:guid}")]
        public async Task<IActionResult> GetCollection(Guid dataTypeUid)
        {
            var result = await _service.GetColllectionAsync(dataTypeUid);
            return Ok(result);
        }

        [HttpGet("{dataTypeUid:guid}/{uid}")]
        public async Task<IActionResult> GetItem(Guid dataTypeUid, string uid)
        {
            var result = await _service.GetItemAsync(dataTypeUid, uid);
            return Ok(result);
        }
    }
}
