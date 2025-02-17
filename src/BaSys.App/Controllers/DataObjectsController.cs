using BaSys.App.Abstractions;
using BaSys.Common.Infrastructure;
using BaSys.Core.Features.Abstractions;
using BaSys.Core.Features.DataObjects.Abstractions;
using BaSys.Core.Features.DataObjects.Queries;
using BaSys.DTO.App;
using BaSys.Host.DAL.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace BaSys.App.Controllers
{
    [Route("api/app/v1/[controller]")]
    [ApiController]
    [Authorize(Roles = ApplicationRole.User)]
    public class DataObjectsController : ControllerBase, IDisposable
    {

        private readonly IDataObjectsService _service;

        private readonly IDataObjectCreateCommandHandler _createCommandHandler;
        private readonly IDataObjectUpdateCommanHandler _updateCommandHandler;
        private readonly IDataObjectRegistratorRouteQueryHandler _queryRouteHandler;
        private readonly IDbConnection _connection;
        private bool _disposed = false;


        public DataObjectsController(IMainConnectionFactory connectionFactory, 
            IDataObjectsService service, 
            IDataObjectCreateCommandHandler createCommandHandler,
            IDataObjectUpdateCommanHandler updateCommandHandler,
            IDataObjectRegistratorRouteQueryHandler queryRouteHandler)
        {
            _connection = connectionFactory.CreateConnection();

            _service = service;

            _createCommandHandler = createCommandHandler;
            _updateCommandHandler = updateCommandHandler;

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
            var result = await _createCommandHandler.SetUp(_connection).ExecuteAsync(dto);

            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateItem([FromBody] DataObjectSaveDto dto)
        {
            var result = await _updateCommandHandler.SetUp(_connection).ExecuteAsync(dto);

            return Ok(result);
        }

        [HttpDelete("{kind}/{name}/{uid}")]
        public async Task<IActionResult> DeleteItem(string kind, string name, string uid)
        {
            var result = await _service.DeleteItemAsync(kind, name, uid);

            return Ok(result);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_connection != null)
                        _connection.Dispose();
                }

                _disposed = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
