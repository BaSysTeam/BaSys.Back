using BaSys.Admin.Abstractions;
using BaSys.Common.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections;

namespace BaSys.Admin.Controllers
{
    [Route("api/admin/v1/[controller]")]
    [ApiController]
    [Authorize(Roles = ApplicationRole.Administrator)]
    public class WorkflowsBoardController : ControllerBase
    {
        private readonly IWorkflowsBoardService _service;

        public WorkflowsBoardController(IWorkflowsBoardService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetInfo()
        {
            var result = await _service.GetInfoAsync();

            return Ok(result);
        }
    }
}
