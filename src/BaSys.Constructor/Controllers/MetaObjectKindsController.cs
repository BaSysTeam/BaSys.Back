using BaSys.Common.Infrastructure;
using BaSys.Constructor.Abstractions;
using BaSys.Constructor.Infrastructure;
using BaSys.Core.Abstractions;
using BaSys.Host.DAL.Abstractions;
using BaSys.Metadata.Models;
using BaSys.SuperAdmin.DAL.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BaSys.Constructor.Controllers
{
    /// <summary>
    /// Provides endpoints for managing metaobject kinds settings.
    /// This controller requires the user to be authenticated and to have Administrator role.
    /// It uses a custom action filter for setting the database name context.
    /// </summary>
    [Route("api/constructor/v1/[controller]")]
    [ApiController]
    [Authorize(Roles = ApplicationRole.Administrator)]
    public class MetaObjectKindsController : ControllerBase
    {

        private readonly IMetaObjectKindsService _metaObjectKindsService;

        /// <summary>
        /// Initializes a new instance of the MetaObjectKindsController class.
        /// </summary>
        /// <param name="metaObjectKindsService">Service for handling metaobject kinds operations.</param>
        public MetaObjectKindsController(IMetaObjectKindsService metaObjectKindsService) 
        {
            _metaObjectKindsService = metaObjectKindsService;
        }

        /// <summary>
        /// Retrieves the collection of metaobject kinds.
        /// </summary>
        /// <returns>An IActionResult containing the collection of metaobject kinds.</returns>
        [HttpGet]
        public async Task<IActionResult> GetCollection()
        {
            var result = await _metaObjectKindsService.GetCollectionAsync();
            return Ok(result);
        }

        /// <summary>
        /// Retrieves the settings collection of metaobject kinds.
        /// </summary>
        /// <returns>An IActionResult containing the collection of metaobject kinds settings.</returns>
        [HttpGet("Settings")]
        public async Task<IActionResult> GetSettingsCollection()
        {
            var result = await _metaObjectKindsService.GetSettingsCollection();
            return Ok(result);
        }

        /// <summary>
        /// Retrieves a specific metaobject kind setting by its unique identifier.
        /// </summary>
        /// <param name="uid">The unique identifier of the metaobject kind setting to retrieve.</param>
        /// <returns>An IActionResult containing the specified metaobject kind setting.</returns>
        [HttpGet("{uid:guid}")]
        public async Task<IActionResult> GetItem(Guid uid)
        {
            var result = await _metaObjectKindsService.GetSettingsItemAsync(uid);

            return Ok(result);
        }

        /// <summary>
        /// Retrieves a specific metaobject kind setting by its unique name.
        /// </summary>
        /// <param name="name">The name of the metaobject kind setting to retrieve.</param>
        /// <returns>An IActionResult containing the specified metaobject kind setting.</returns>
        [HttpGet("{name}")]
        public async Task<IActionResult> GetItem(string name)
        {
            var result = await _metaObjectKindsService.GetSettingsItemByNameAsync(name);

            return Ok(result);
        }

        /// <summary>
        /// Creates a new metaobject kind setting.
        /// </summary>
        /// <param name="settings">The metaobject kind settings to create.</param>
        /// <returns>An IActionResult containing the newly created metaobject kind setting.</returns>
        [HttpPost]
        public async Task<IActionResult> CreateItem(MetaObjectKindSettings settings)
        {
            var result = await _metaObjectKindsService.InsertSettingsAsync(settings);

            return Ok(result);

        }

        /// <summary>
        /// Creates standard meta object kinds.
        /// </summary>
        /// <returns>An IActionResult containing info about created item count.</returns>
        [HttpPost("Standard")]
        public async Task<IActionResult> CreateStandardItems()
        {
            var result = await _metaObjectKindsService.InsertStandardItemsAsync();

            return Ok(result);

        }

        /// <summary>
        /// Updates an existing metaobject kind setting.
        /// </summary>
        /// <param name="settings">The metaobject kind settings to update.</param>
        /// <returns>An IActionResult indicating the success or failure of the update operation.</returns>
        [HttpPut]
        public async Task<IActionResult> UpdateItem(MetaObjectKindSettings settings)
        {
            var result = await _metaObjectKindsService.UpdateSettingsAsync(settings);

            return Ok(result);

        }

        /// <summary>
        /// Deletes a metaobject kind setting by its unique identifier.
        /// </summary>
        /// <param name="uid">The unique identifier of the metaobject kind setting to delete.</param>
        /// <returns>An IActionResult indicating the success or failure of the delete operation.</returns>
        [HttpDelete("{uid}")]
        public async Task<IActionResult> DeleteItem(Guid uid)
        {
            var result = await _metaObjectKindsService.DeleteAsync(uid);

            return Ok(result);
        }

    }
}
