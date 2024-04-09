using BaSys.Admin.Abstractions;
using BaSys.Admin.DTO;
using BaSys.Admin.Services;
using BaSys.Common.Enums;
using BaSys.Common.Infrastructure;
using BaSys.DTO.Admin;
using BaSys.Logging.Abstractions.Abstractions;
using BaSys.Logging.EventTypes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BaSys.Admin.Controllers
{
    /// <summary>
    /// This controller allow to manipulate with app constants records.
    /// Implemented CRUD operations.
    /// </summary>
    [Route("api/admin/v1/[controller]")]
    [ApiController]
    [Authorize(Roles = ApplicationRole.Administrator)]
    public class AppConstantsController : ControllerBase
    {
        private readonly IAppConstantsService _appConstantsService;
        private readonly IBaSysLoggerFactory _loggerFactory;

        public AppConstantsController(IAppConstantsService appConstantsService,
            IBaSysLoggerFactory loggerFactory)
        {
            _appConstantsService = appConstantsService;
            _loggerFactory = loggerFactory;
        }

        /// <summary>
        /// Retrieve app constants item.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAppConstants()
        {
            var dbName = GetDbName();
            var result = await _appConstantsService.GetAppConstantsAsync(dbName);

            return Ok(result);
        }

        /// <summary>
        /// Update app constants item.
        /// </summary>
        /// <param name="appConstantsRecord"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IActionResult> UpdateAppConstants(AppConstantsDto appConstantsRecord)
        {
            var dbName = GetDbName();
            var result = await _appConstantsService.UpdateAppConstantsAsync(appConstantsRecord, dbName);
            
            using var logger = await _loggerFactory.GetLogger();
            logger.Write("UpdateAppConstants", EventTypeLevels.Info, new SettingsChangedEventType());

            return Ok(result);
        }

        private string? GetDbName()
        {
            var authUserDbNameClaim = User.Claims.FirstOrDefault(x => x.Type == "DbName");
            return authUserDbNameClaim?.Value;
        }
    }
}
