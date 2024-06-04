using BaSys.Admin.Abstractions;
using BaSys.Common.Enums;
using BaSys.Common.Infrastructure;
using BaSys.DTO.Admin;
using BaSys.Logging.Abstractions.Abstractions;
using BaSys.Logging.EventTypes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BaSys.Admin.Controllers
{
    /// <summary>
    /// This controller allow to manipulate with app constants record.
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
        /// Retrieve app constants record.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAppConstants()
        {
            var result = await _appConstantsService.GetAppConstantsAsync();
            return Ok(result);
        }

        /// <summary>
        /// Update app constants record.
        /// </summary>
        /// <param name="appConstantsRecord"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IActionResult> UpdateAppConstants(AppConstantsDto appConstantsRecord)
        {
            var result = await _appConstantsService.UpdateAppConstantsAsync(appConstantsRecord);
            
            using var logger = await _loggerFactory.GetLogger();
            logger.Write("UpdateAppConstants", EventTypeLevels.Info, new SettingsChangedEventType());

            return Ok(result);
        }
    }
}
