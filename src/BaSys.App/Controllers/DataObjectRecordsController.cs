using BaSys.App.Abstractions;
using BaSys.Common.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BaSys.App.Controllers
{
    [Route("api/app/v1/[controller]")]
    [ApiController]
    [Authorize(Roles = ApplicationRole.User)]
    public class DataObjectRecordsController : ControllerBase
    {
        private readonly IDataObjectRecordsService _recordsService;

        public DataObjectRecordsController(IDataObjectRecordsService recordsService)
        {
            _recordsService = recordsService;
        }

        [HttpGet("Model/{kind}/{name}/{uid}")]
        public async Task<IActionResult> GetModel(string kind, string name, string uid)
        {
            var result = await _recordsService.GetModelAsync(kind, name, uid);

            return Ok(result);
        }

        [HttpGet("Records/{kind}/{name}/{objectUid}/{registerUid:guid}")]
        public async Task<IActionResult> GetRecords(string kind, string name, string objectUid, Guid registerUid)
        {
            var result = await _recordsService.GetRecordsAsync(kind, name, objectUid, registerUid);

            return Ok(result);
        }

    }
}
