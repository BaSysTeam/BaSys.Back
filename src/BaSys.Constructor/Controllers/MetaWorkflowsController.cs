using BaSys.Common.Infrastructure;
using BaSys.Core.Abstractions;
using BaSys.DTO.Constructor;
using BaSys.Host.DAL.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace BaSys.Constructor.Controllers
{
    [Route("api/constructor/v1/[controller]")]
    [ApiController]
    [Authorize(Roles = ApplicationRole.Designer)]
    public class MetaWorkflowsController : ControllerBase, IDisposable
    {
        private readonly IMetaWorkflowsService _service;
        private readonly IDbConnection _connection;
        private bool _disposed = false;

        public MetaWorkflowsController(IMainConnectionFactory connectionFactory, IMetaWorkflowsService service)
        {
            _connection = connectionFactory.CreateConnection();

            _service = service;
            _service.SetUp(_connection);
        }

        [HttpGet()]
        public async Task<IActionResult> GetList()
        {
            var result = await _service.GetListAsync();

            return Ok(result);
        }

        [HttpGet("{name}")]
        public async Task<IActionResult> GetMetaObject(string name)
        {
            var result = await _service.GetSettingsItemAsync(name);

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMetaObject(WorkflowSettingsDto settings)
        {
            var result = await _service.CreateAsync(settings);

            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateMetaObject(WorkflowSettingsDto settings)
        {
            var result = await _service.UpdateSettingsItemAsync(settings);
            return Ok(result);
        }

        [HttpDelete("{name}")]
        public async Task<IActionResult> DeleteObject(string name)
        {
            var result = await _service.DeleteAsync(name);
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
