using BaSys.Common.Infrastructure;
using BaSys.Core.Abstractions;
using BaSys.Host.DAL.Abstractions;
using BaSys.Workflows.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace BaSys.App.Controllers
{
    [Route("api/app/v1/[controller]")]
    [ApiController]
    [Authorize(Roles = ApplicationRole.User)]
    public class WorkflowsController : ControllerBase, IDisposable
    {
        private readonly IWorkflowsService _service;
        private readonly IDbConnection _connection;
        private bool _disposed = false;

        public WorkflowsController(IMainConnectionFactory connectionFactory, 
            IWorkflowsService service)
        {
            _connection = connectionFactory.CreateConnection();

            _service = service;
            _service.SetUp(_connection);
        }

        [HttpPost("start")]
        public async Task<IActionResult> Start([FromBody] WorkflowStartDto startDto)
        {
            var result = await _service.StartAsync(startDto);

            return Ok(result);
        }

        [HttpGet("check/{runUid}")]
        public async Task<IActionResult> Check(string runUid)
        {
            var result = await _service.CheckAsync(runUid);

            return Ok(result);
        }

        [HttpDelete("{runUid}")]
        public async Task<IActionResult> Terminate(string runUid)
        {
            var result = await _service.TerminateAsync(runUid);

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
