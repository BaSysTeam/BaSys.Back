using BaSys.Common.Infrastructure;
using BaSys.Core.Abstractions;
using BaSys.Host.DAL.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using WorkflowCore.Interface;

namespace BaSys.App.Controllers
{
    [Route("api/app/v1/[controller]")]
    [ApiController]
    [Authorize(Roles = ApplicationRole.User)]
    public class WorkflowsController : ControllerBase, IDisposable
    {
        private readonly IWorkflowsService _service;
        private readonly IDbConnection _connection;
        private readonly IWorkflowHost _host;
        private bool _disposed = false;

        public WorkflowsController(IMainConnectionFactory connectionFactory, 
            IWorkflowsService service, 
            IWorkflowHost host)
        {
            _connection = connectionFactory.CreateConnection();
            _host = host;

            _service = service;
            _service.SetUp(_connection);
        }

        [HttpPost("start/{name}")]
        public async Task<IActionResult> Start(string name)
        {
            var result = await _service.StartAsync(name);

            return Ok(result);
        }

        [HttpGet("check/{runUid}")]
        public async Task<IActionResult> Check(string runUid)
        {
            var result = await _service.CheckAsync(runUid);

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
