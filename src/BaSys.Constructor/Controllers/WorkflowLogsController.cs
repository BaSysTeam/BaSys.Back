using BaSys.Common.Infrastructure;
using BaSys.Constructor.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BaSys.Constructor.Controllers
{
    [Route("api/constructor/v1/[controller]")]
    [ApiController]
    [Authorize(Roles = ApplicationRole.Designer)]
    public class WorkflowLogsController : ControllerBase
    {
        private readonly IWorkflowLogsService _service;

        public WorkflowLogsController(IWorkflowLogsService service)
        {
            _service = service;
        }

        [HttpGet("lifecycle/{workflowUid}")]
        public async Task<IActionResult> GetWorkflowLifecycleRecords(string workflowUid)
        {
            var result = await _service.GetLifecycleRecordsAsync(workflowUid);

            return Ok(result);
        }


        [HttpGet("run/{runUid}")]
        public async Task<IActionResult> GetRecordsByRun(string runUid)
        {
            var result = await _service.GetRecordsByRunAsync(runUid);

            return Ok(result);
        }

        [HttpDelete("{workflowUid}")]
        public async Task<IActionResult> DeleteWorkflowRecords(string workflowUid)
        {
            var result = await _service.DeleteWorkflowRecordsAsync(workflowUid);

            return Ok(result);
        }
    }
}
