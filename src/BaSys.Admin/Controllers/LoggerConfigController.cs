using BaSys.Admin.Abstractions;
using BaSys.Common.Infrastructure;
using BaSys.DTO.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
        public async Task<IActionResult> GetLoggerConfig()
        {
            var dbName = GetDbName();
            var result = await _loggerConfigService.GetLoggerConfigAsync(dbName);

            return Ok(result);
        }

        /// <summary>
        /// Update logger config record.
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IActionResult> UpdateLoggerConfig(LoggerConfigDto dto)
        {
            var dbName = GetDbName();
            var result = await _loggerConfigService.UpdateLoggerConfigAsync(dto, dbName);

            return Ok(result);
        }

        private string? GetDbName()
        {
            var authUserDbNameClaim = User.Claims.FirstOrDefault(x => x.Type == "DbName");
            return authUserDbNameClaim?.Value;
        }
    }
}
