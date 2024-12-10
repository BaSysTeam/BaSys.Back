using BaSys.App.Abstractions;
using BaSys.App.Features.DataObjectRecords.Queries;
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
       
        private readonly IGetRecordsQueryHandler _getRecordsHandler;
        private readonly IGetRecordsDialogModelQueryHandler _getModelHandler;

        public DataObjectRecordsController(IGetRecordsDialogModelQueryHandler getModelHandler,
            IGetRecordsQueryHandler getRecordsQueryHandler)
        {
            _getModelHandler = getModelHandler;
            _getRecordsHandler = getRecordsQueryHandler;
        }

        [HttpGet("Model/{kind}/{name}/{uid}")]
        public async Task<IActionResult> GetModel(string kind, string name, string uid)
        {
            var result = await _getModelHandler.ExecuteAsync(new GetRecordsDialogModelQuery(kind, name, uid));

            return Ok(result);
        }

        [HttpGet("Records/{kind}/{name}/{objectUid}/{registerUid:guid}")]
        public async Task<IActionResult> GetRecords(string kind, string name, string objectUid, Guid registerUid)
        {
          
            var result = await _getRecordsHandler.ExecuteAsync(new GetRecordsQuery(kind, name, objectUid, registerUid));

            return Ok(result);
        }

    }
}
