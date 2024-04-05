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
    [Authorize]
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
            var authUserDbNameClaim = User.Claims.FirstOrDefault(x => x.Type == "DbName");
            var result = await _appConstantsRecordsService.GetAppConstantsRecordAsync(authUserDbNameClaim?.Value);

            return Ok(result);
        }

        /// <summary>
        /// Creates new app constants record.
        /// </summary>
        /// <param name="appConstantsRecord"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateAppConstantsRecord(AppConstantsRecordDto appConstantsRecord)
        {
            var authUserDbNameClaim = User.Claims.FirstOrDefault(x => x.Type == "DbName");
            var result = await _appConstantsRecordsService.CreateAppConstantsRecordAsync(appConstantsRecord, authUserDbNameClaim?.Value);

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
            var authUserDbNameClaim = User.Claims.FirstOrDefault(x => x.Type == "DbName");
            var result = await _appConstantsRecordsService.UpdateAppConstantsRecordAsync(appConstantsRecord, authUserDbNameClaim?.Value);

            return Ok(result);
        }

        /// <summary>
        /// Delete app constants record by Uid.
        /// </summary>
        /// <param name="uid">Uid</param>
        /// <returns></returns>
        [HttpDelete("{uid}")]
        public async Task<IActionResult> DeleteAppConstantsRecord(Guid uid)
        {
            var authUserDbNameClaim = User.Claims.FirstOrDefault(x => x.Type == "DbName");
            var result = await _appConstantsRecordsService.DeleteAppConstantsRecordAsync(uid, authUserDbNameClaim?.Value);

            return Ok(result);
        }
    }
}
