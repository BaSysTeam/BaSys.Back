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
    [Route("api/constructor/v1/[controller]")]
    [ApiController]
    [Authorize(Roles = ApplicationRole.Administrator)]
    [DbNameActionFilter]
    public class MetadataKindsController : ApiControllerBase
    {

        private readonly IMetadataKindsService _metadataKindsService;

        public MetadataKindsController(
            IMetadataKindsService metadataKindsService)
        {
            _metadataKindsService = metadataKindsService;
        }

        [HttpGet]
        public async Task<IActionResult> GetSettingsCollection()
        {
            _metadataKindsService.SetUp(DbName);
            var result = await _metadataKindsService.GetSettingsCollectionAsync(null);

            return Ok(result);
        }

        [HttpGet("{uid}")]
        public async Task<IActionResult> GetItem(Guid uid)
        {
            _metadataKindsService.SetUp(DbName);
            var result = await _metadataKindsService.GetSettingsItemAsync(uid, null);

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateItem(MetadataKindSettings settings)
        {
            _metadataKindsService.SetUp(DbName);
            var result = await _metadataKindsService.InsertSettingsAsync(settings, null);

            return Ok(result);

        }

        [HttpPut]
        public async Task<IActionResult> UpdateItem(MetadataKindSettings settings)
        {
            _metadataKindsService.SetUp(DbName);
            var result = await _metadataKindsService.UpdateSettingsAsync(settings, null);

            return Ok(result);

        }

        [HttpDelete("{uid}")]
        public async Task<IActionResult> DeleteItem(Guid uid)
        {
            _metadataKindsService.SetUp(DbName);
            var result = await _metadataKindsService.DeleteAsync(uid, null);

            return Ok(result);
        }

    }
}
