using BaSys.Common.Infrastructure;
using BaSys.Core.Abstractions;
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
    public class WorkflowsController : ControllerBase
    {
        private readonly IWorkflowsService _service;
        private readonly IDbConnection _connection;
        private bool _disposed = false;

        public WorkflowsController(IMainConnectionFactory connectionFactory, IWorkflowsService service)
        {
            _connection = connectionFactory.CreateConnection();

            _service = service;
            _service.SetUp(_connection);
        }

        [HttpPost("start/{name}")]
        public async Task<IActionResult> Start(string name)
        {
            var result = await _service.StartAsync(name);

            return Ok(result);
        }
    }
}
