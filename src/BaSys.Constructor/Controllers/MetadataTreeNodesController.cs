using BaSys.Common.Infrastructure;
using BaSys.Constructor.Abstractions;
using BaSys.Metadata.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BaSys.Constructor.Controllers
{
    [Route("api/constructor/v1/[controller]")]
    [ApiController]
    [Authorize(Roles = ApplicationRole.Designer)]
    public class MetadataTreeNodesController : ControllerBase
    {
        private readonly IMetadataTreeNodesService _metadataTreeNodesService;

        public MetadataTreeNodesController(IMetadataTreeNodesService metadataTreeNodesService)
        {
            _metadataTreeNodesService = metadataTreeNodesService;
        }

        /// <summary>
        /// Retrieves standard nodes of the metadata tree.
        /// </summary>
        /// <returns>
        /// An IActionResult containing the collection of metadata tree nodes.
        /// </returns>
        [HttpGet("Standard")]
        public async Task<IActionResult> GetStandard()
        {
            var result = await _metadataTreeNodesService.GetStandardNodesAsync();
            return Ok(result);
        }

        /// <summary>
        /// Retrieves children of metadata tree node by its unique identifier.
        /// </summary>
        /// <param name="uid">The unique identifier of the metadata tree node to get children.</param>
        /// <returns>
        /// An IActionResult containing the collection of metadata tree nodes.
        /// </returns>
        [HttpGet("Children/{uid}")]
        public async Task<IActionResult> GetChildren(Guid uid)
        {
            var result = await _metadataTreeNodesService.GetChildrenAsync(uid);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves nodes of the metadata tree which are groups.
        /// </summary>
        /// <returns>
        /// An IActionResult containing the collection of metadata tree nodes.
        /// </returns>
        [HttpGet("Groups")]
        public async Task<IActionResult> GetGroups()
        {
            var result = await _metadataTreeNodesService.GetGroupsAsync();
            return Ok(result);
        }

        /// <summary>
        /// Inserts new metadata tree node.
        /// </summary>
        /// <param name="dto">The metadata tree node to create.</param>
        /// <returns>An IActionResult containing the newly created metadata tree node.</returns>
        [HttpPost]
        public async Task<IActionResult> Create(MetadataTreeNodeDto dto)
        {
            var result = await _metadataTreeNodesService.InsertAsync(dto);
            return Ok(result);
        }

        /// <summary>
        /// Updates metadata tree node.
        /// </summary>
        /// <param name="dto">
        /// The metadata tree node to update.
        /// </param>
        /// <returns>
        /// An IActionResult containing the updated metadata tree node.
        /// </returns>
        [HttpPut]
        public async Task<IActionResult> Update(MetadataTreeNodeDto dto)
        {
            var result = await _metadataTreeNodesService.UpdateAsync(dto);
            return Ok(result);
        }

        /// <summary>
        /// Creates new meta object.
        /// </summary>
        /// <param name="dto">The info about new meta object to create.</param>
        /// <returns>An IActionResult containing the newly created metadata tree node.</returns>
        [HttpPost("MetaObject")]
        public async Task<IActionResult> CreateMetaObject(CreateMetaObjectDto dto)
        {
            var result = await _metadataTreeNodesService.InsertMetaObjectAsync(dto);
            return Ok(result);
        }

        /// <summary>
        /// Deletes metadata tree node.
        /// </summary>
        /// <param name="dto">The model of the metadata tree node to delete.</param>
        /// <returns>An IActionResult indicating the success or failure of the delete operation.</returns>
        [HttpDelete]
        public async Task<IActionResult> Delete(MetadataTreeNodeDto dto)
        {
            var result = await _metadataTreeNodesService.DeleteAsync(dto);
            return Ok(result);
        }
    }
}
