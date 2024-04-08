using BaSys.Admin.Abstractions;
using BaSys.Admin.DTO;
using BaSys.Admin.Services;
using BaSys.Common.Infrastructure;
using BaSys.Common.Models;
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
    public class AppConstantsRecordsController : ControllerBase
    {
        private readonly IAppConstantsRecordsService _appConstantsRecordsService;

        public AppConstantsRecordsController(IAppConstantsRecordsService appConstantsRecordsService)
        {
            _appConstantsRecordsService = appConstantsRecordsService;
        }

        /// <summary>
        /// Retrieve app constants record.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAppConstantsRecord()
        {
            var dbName = GetDbName();
            var result = await _appConstantsRecordsService.GetAppConstantsRecordAsync(dbName);

            return Ok(result);
        }

        /// <summary>
        /// Update app constants record.
        /// </summary>
        /// <param name="appConstantsRecord"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IActionResult> UpdateAppConstantsRecord(AppConstantsRecordDto appConstantsRecord)
        {
            var dbName = GetDbName();
            var result = await _appConstantsRecordsService.UpdateAppConstantsRecordAsync(appConstantsRecord, dbName);

            return Ok(result);
        }

        private string? GetDbName()
        {
            var authUserDbNameClaim = User.Claims.FirstOrDefault(x => x.Type == "DbName");
            return authUserDbNameClaim?.Value;
        }
    }
}
