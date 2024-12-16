using BaSys.App.Abstractions;
using BaSys.App.Features.DataObjectRecords.Commands;
using BaSys.App.Features.DataObjectRecords.Queries;
using BaSys.Common.Enums;
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
        private readonly ICreateRecordsCommandHandler _createRecordsHandler;
        private readonly IDeleteRecordsCommandHandler _deleteRecordsHandler;

        public DataObjectRecordsController(IGetRecordsDialogModelQueryHandler getModelHandler,
            IGetRecordsQueryHandler getRecordsQueryHandler, 
            ICreateRecordsCommandHandler createRecordsHandler, 
            IDeleteRecordsCommandHandler deleteRecordsHandler)
        {
            _getModelHandler = getModelHandler;
            _getRecordsHandler = getRecordsQueryHandler;
            _createRecordsHandler = createRecordsHandler;
            _deleteRecordsHandler = deleteRecordsHandler;
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

        [HttpPost("{kind}/{name}/{uid}")]
        public async Task<IActionResult> CreateRecords(string kind, string name, string uid, [FromQuery] EventTypeLevels logLevel)
        {
            var result = await _createRecordsHandler.ExecuteAsync(new CreateRecordsCommand(kind, name, uid, logLevel));

            return Ok(result);
        }

        [HttpDelete("{kind}/{name}/{uid}")]
        public async Task<IActionResult> DeleteRecords(string kind, string name, string uid)
        {
            var result = await _deleteRecordsHandler.ExecuteAsync(new DeleteRecordsCommand(kind, name, uid));

            return Ok(result);
        }

    }
}
