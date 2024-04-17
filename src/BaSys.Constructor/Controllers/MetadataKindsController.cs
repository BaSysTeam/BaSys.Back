using BaSys.Common.Infrastructure;
using BaSys.Constructor.Abstractions;
using BaSys.Constructor.Infrastructure;
using BaSys.Host.DAL.Abstractions;
using BaSys.Metadata.Models;
using BaSys.SuperAdmin.DAL.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BaSys.Constructor.Controllers
{
    /// <summary>
    /// Provides endpoints for managing metadata kinds settings.
    /// This controller requires the user to be authenticated and to have Administrator role.
    /// It uses a custom action filter for setting the database name context.
    /// </summary>
    [Route("api/constructor/v1/[controller]")]
    [ApiController]
    [Authorize(Roles = ApplicationRole.Administrator)]
    [DbNameActionFilter]
    public class MetadataKindsController : ApiControllerBase
    {

        private readonly IMetadataKindsService _metadataKindsService;

        /// <summary>
        /// Initializes a new instance of the MetadataKindsController class.
        /// </summary>
        /// <param name="connectionFactory">Provides functionality to create database connections.</param>
        /// <param name="dbInfoRecordsProvider">Provides database information records.</param>
        /// <param name="metadataKindsService">Service for handling metadata kinds operations.</param>
        public MetadataKindsController(IBaSysConnectionFactory connectionFactory, 
            IDbInfoRecordsProvider dbInfoRecordsProvider, 
            IMetadataKindsService metadataKindsService) :base(connectionFactory, dbInfoRecordsProvider)
        {
            _metadataKindsService = metadataKindsService;
        }

        /// <summary>
        /// Retrieves the collection of metadata kinds settings.
        /// </summary>
        /// <returns>An IActionResult containing the collection of settings.</returns>
        [HttpGet]
        public async Task<IActionResult> GetSettingsCollection()
        {
            var result = await _metadataKindsService.SetUp(_connection).GetCollectionAsync(null);

            return Ok(result);
        }

        /// <summary>
        /// Retrieves a specific metadata kind setting by its unique identifier.
        /// </summary>
        /// <param name="uid">The unique identifier of the metadata kind setting to retrieve.</param>
        /// <returns>An IActionResult containing the specified metadata kind setting.</returns>
        [HttpGet("{uid}")]
        public async Task<IActionResult> GetItem(Guid uid)
        {
            var result = await _metadataKindsService.SetUp(_connection).GetSettingsItemAsync(uid, null);

            return Ok(result);
        }

        /// <summary>
        /// Creates a new metadata kind setting.
        /// </summary>
        /// <param name="settings">The metadata kind settings to create.</param>
        /// <returns>An IActionResult containing the newly created metadata kind setting.</returns>
        [HttpPost]
        public async Task<IActionResult> CreateItem(MetadataKindSettings settings)
        {
            var result = await _metadataKindsService.SetUp(_connection).InsertSettingsAsync(settings, null);

            return Ok(result);

        }

        /// <summary>
        /// Updates an existing metadata kind setting.
        /// </summary>
        /// <param name="settings">The metadata kind settings to update.</param>
        /// <returns>An IActionResult indicating the success or failure of the update operation.</returns>
        [HttpPut]
        public async Task<IActionResult> UpdateItem(MetadataKindSettings settings)
        {
            var result = await _metadataKindsService.SetUp(_connection).UpdateSettingsAsync(settings, null);

            return Ok(result);

        }

        /// <summary>
        /// Deletes a metadata kind setting by its unique identifier.
        /// </summary>
        /// <param name="uid">The unique identifier of the metadata kind setting to delete.</param>
        /// <returns>An IActionResult indicating the success or failure of the delete operation.</returns>
        [HttpDelete("{uid}")]
        public async Task<IActionResult> DeleteItem(Guid uid)
        {
            var result = await _metadataKindsService.SetUp(_connection).DeleteAsync(uid, null);

            return Ok(result);
        }

    }
}
