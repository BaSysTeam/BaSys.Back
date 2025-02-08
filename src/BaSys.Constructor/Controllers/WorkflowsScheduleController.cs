using BaSys.Common.Infrastructure;
using BaSys.Host.DAL.Abstractions;
using BaSys.Metadata.Models.WorkflowModel;
using BaSys.Workflows.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace BaSys.Constructor.Controllers
{
    [Route("api/constructor/v1/[controller]")]
    [ApiController]
    [Authorize(Roles = ApplicationRole.Designer)]
    public class WorkflowsScheduleController : ControllerBase, IDisposable
    {
        private readonly IWorkflowsScheduleService _service;
        private readonly IDbConnection _connection;
        private bool _disposed = false;

        public WorkflowsScheduleController(IMainConnectionFactory connectionFactory, IWorkflowsScheduleService service)
        {
            _connection = connectionFactory.CreateConnection();

            _service = service;
            _service.SetUp(_connection);
        }

        [HttpGet]
        public async Task<IActionResult> GetCollection([FromQuery] Guid? workflowUid, [FromQuery] bool? isActive)
        {
            var result = await _service.GetCollectionAsync(workflowUid, isActive);

            return Ok(result);
        }

        [HttpGet("{uid:guid}")]
        public async Task<IActionResult> GetItem(Guid uid)
        {
            var result = await _service.GetRecordAsync(uid);

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(WorkflowScheduleRecord record)
        {
            var result = await _service.CreateAsync(record);

            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> Update(WorkflowScheduleRecord record)
        {
            var result = await _service.UpdateAsync(record);
            return Ok(result);
        }

        [HttpDelete("{uid:guid}")]
        public async Task<IActionResult> DeleteObject(Guid uid)
        {
            var result = await _service.DeleteAsync(uid);
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
