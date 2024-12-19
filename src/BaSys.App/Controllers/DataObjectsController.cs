using BaSys.App.Abstractions;
using BaSys.Common.Infrastructure;
using BaSys.Core.Features.Abstractions;
using BaSys.Core.Features.DataObjects.Queries;
using BaSys.DTO.App;
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
        private readonly IDataObjectRegistratorRouteQueryHandler _queryRouteHandler;

        public DataObjectsController(IDataObjectsService service, IDataObjectRegistratorRouteQueryHandler queryRouteHandler)
        {
            _service = service;
            _queryRouteHandler = queryRouteHandler;
        }

        [HttpGet("{kind}/{name}")]
        public async Task<IActionResult> GetCollection(string kind, string name)
        {
            var result =  await _service.GetCollectionAsync(kind, name);

            return Ok(result);
        }

        [HttpGet("{kind}/{name}/{uid}")]
        public async Task<IActionResult> GetItem(string kind, string name, string uid)
        {
            var result = await _service.GetItemAsync(kind, name, uid);

            return Ok(result);
        }

        [HttpPost("registrator-route")]
        public async Task<IActionResult> QueryRegistratorRoute([FromBody] DataObjectRegistratorRouteQuery dto)
        {
            var result = await _queryRouteHandler.ExecuteAsync(dto);
            return Ok(result);
        }

        [HttpGet("{kind}/{name}/{uid}/details/{tableName}")]
        public async Task<IActionResult> GetDetailsTable(string kind, string name, string uid, string tableName)
        {
            var result = await _service.GetDetailsTableAsync(kind, name, uid, tableName);

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateItem([FromBody]DataObjectSaveDto dto)
        {
            var result = await _service.InsertAsync(dto);

            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateItem([FromBody] DataObjectSaveDto dto)
        {
            var result = await _service.UpdateAsync(dto);

            return Ok(result);
        }

        [HttpDelete("{kind}/{name}/{uid}")]
        public async Task<IActionResult> DeleteItem(string kind, string name, string uid)
        {
            var result = await _service.DeleteItemAsync(kind, name, uid);

            return Ok(result);
        }
    }
}
