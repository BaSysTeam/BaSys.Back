using BaSys.Admin.Abstractions;
using BaSys.Common.Infrastructure;
using BaSys.Workflows.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BaSys.Admin.Controllers
{
    [Route("api/admin/v1/[controller]")]
    [ApiController]
    [Authorize(Roles = ApplicationRole.Administrator)]
    public class WorkflowsBoardController : ControllerBase
    {
        private readonly IWorkflowsBoardService _service;
        private readonly IWorkflowTerminateCommandHandler _terminateHandler;


        public WorkflowsBoardController(IWorkflowsBoardService service, 
            IWorkflowTerminateCommandHandler terminateHandler)
        {
            _service = service;
            _terminateHandler = terminateHandler;
        }

        [HttpGet]
        public async Task<IActionResult> GetInfo()
        {
            var result = await _service.GetInfoAsync();

            return Ok(result);
        }

        [HttpDelete("{runUid}")]
        public async Task<IActionResult> Terminate(string runUid)
        {
            var result = await _terminateHandler.ExecuteAsync(runUid);

            return Ok(result);
        }
    }
}
