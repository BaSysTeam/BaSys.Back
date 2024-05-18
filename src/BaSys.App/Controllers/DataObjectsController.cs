using BaSys.App.Abstractions;
using BaSys.Common.Infrastructure;
using BaSys.Host.DAL.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BaSys.App.Controllers
{
    [Route("api/app/v1/[controller]")]
    [ApiController]
    [Authorize(Roles = ApplicationRole.User)]
    public class DataObjectsController : ControllerBase
    {

        private readonly IDataObjectsService _service;

        public DataObjectsController(IDataObjectsService service)
        {
            _service = service;
        }

        [HttpGet("{kind}/{name}")]
        public async Task<IActionResult> GetCollection(string kind, string name)
        {
            var result =  await _service.GetCollectionAsync(kind, name);

            return Ok(result);
        }
    }
}
