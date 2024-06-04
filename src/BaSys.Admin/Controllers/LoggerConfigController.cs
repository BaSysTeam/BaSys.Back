using BaSys.Admin.Abstractions;
using BaSys.Common.Enums;
using BaSys.Common.Infrastructure;
using BaSys.DAL.Models.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BaSys.Admin.Controllers
{
    /// <summary>
    /// This controller allow to manipulate with logger config record.
    /// </summary>
    [Route("api/admin/v1/[controller]")]
    [ApiController]
    [Authorize(Roles = ApplicationRole.Administrator)]
    public class LoggerConfigController : ControllerBase
    {
        private readonly ILoggerConfigService _loggerConfigService;

        public LoggerConfigController(ILoggerConfigService loggerConfigService)
        {
            _loggerConfigService = loggerConfigService;
        }

        /// <summary>
        /// Retrieve logger config record.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetCurrentLoggerConfig()
        {
            var result = await _loggerConfigService.GetCurrentLoggerConfigAsync();
            return Ok(result);
        }
        
        [HttpGet("GetLoggerConfigByType")]
        public async Task<IActionResult> GetLoggerConfigByType([FromQuery]int loggerType)
        {
            var result = await _loggerConfigService.GetLoggerConfigByTypeAsync((LoggerTypes)loggerType);
            return Ok(result);
        }

        /// <summary>
        /// Update logger config record.
        /// </summary>
        /// <param name="loggerConfig"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IActionResult> UpdateLoggerConfig(LoggerConfig loggerConfig)
        {
            var result = await _loggerConfigService.UpdateLoggerConfigAsync(loggerConfig);
            return Ok(result);
        }
    }
}
