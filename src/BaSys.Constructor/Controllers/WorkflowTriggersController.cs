using BaSys.Common.Infrastructure;
using BaSys.Host.DAL.Abstractions;
using BaSys.Workflows.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace BaSys.Constructor.Controllers
{
    [Route("api/constructor/v1/[controller]")]
    [ApiController]
    [Authorize(Roles = ApplicationRole.Designer)]
    public class WorkflowTriggersController : ControllerBase
    {
        private readonly IWorkflowTriggersService _service;
        private readonly IDbConnection _connection;
        private bool _disposed = false;

        public WorkflowTriggersController(IMainConnectionFactory connectionFactory, IWorkflowTriggersService service)
        {
            _connection = connectionFactory.CreateConnection();

            _service = service;
            _service.SetUp(_connection);
        }

        [HttpGet]
        public async Task<IActionResult> GetCollection([FromQuery] Guid? metaObjectUid, [FromQuery] Guid? workflowUid)
        {
            var result = await _service.GetCollectionAsync(metaObjectUid, workflowUid);

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
