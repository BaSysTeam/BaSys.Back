using BaSys.Common.Infrastructure;
using BaSys.Constructor.Abstractions;
using BaSys.DTO.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BaSys.Constructor.Controllers
{
    [Route("api/constructor/v1/[controller]")]
    [ApiController]
    [Authorize(Roles = ApplicationRole.Designer)]
    public class QueriesController : ControllerBase
    {
        private readonly IQueriesService _queriesService;

        public QueriesController(IQueriesService queriesService)
        {
            _queriesService = queriesService;
        }

        [HttpPost("Execute")]
        public async Task<IActionResult> ExecuteQuery([FromBody] SelectQueryModelDto queryModelDto)
        {
            var result = await _queriesService.ExecuteAsync(queryModelDto);

            return Ok(result);
        }
    }
}
