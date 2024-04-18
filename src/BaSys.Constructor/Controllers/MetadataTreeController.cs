using BaSys.Common.Enums;
using BaSys.Common.Infrastructure;
using BaSys.Constructor.Abstractions;
using BaSys.Metadata.DTOs;
using BaSys.Metadata.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;

namespace BaSys.Constructor.Controllers
{
    [Route("api/constructor/v1/[controller]")]
    [ApiController]
    [Authorize(Roles = ApplicationRole.Administrator)]
    public class MetadataTreeController : ControllerBase
    {
        private readonly IMetadataTreeService _metadataTreeService;

        public MetadataTreeController(IMetadataTreeService metadataTreeService)
        {
            _metadataTreeService = metadataTreeService;
        }

        /// <summary>
        /// Retrieves default nodes of the top level of the metadata tree.
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetDefaultNodes")]
        public async Task<IActionResult> GetDefaultNodes()
        {
            var dbName = GetDbName();
            var result = await _metadataTreeService.GetDefaultNodesAsync(dbName);

            return Ok(result);
        }

        /// <summary>
        /// Adds new metadata group.
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost("AddMetadataGroup")]
        public async Task<IActionResult> AddMetadataGroup(MetadataGroupDto dto)
        {
            var dbName = GetDbName();
            var result = await _metadataTreeService.AddAsync(dto, dbName);

            return Ok(result);
        }

        /// <summary>
        /// Retrieves children of metadata group.
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        [HttpGet("Children/{uid}")]
        public async Task<IActionResult> GetChildren(string uid)
        {
            var dbName = GetDbName();
            var result = await _metadataTreeService.GetChildrenAsync(uid, dbName);

            return Ok(result);
        }

        /// <summary>
        /// Deletes metadata group.
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        [HttpDelete("MetadataGroup/{uid}")]
        public async Task<IActionResult> DeleteMetadataGroup(string uid)
        {
            var dbName = GetDbName();
            var result = await _metadataTreeService.DeleteAsync(uid, dbName);

            return Ok(result);
        }

        private string? GetDbName()
        {
            var authUserDbNameClaim = User.Claims.FirstOrDefault(x => x.Type == "DbName");
            return authUserDbNameClaim?.Value;
        }
    }
}
