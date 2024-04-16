using BaSys.Admin.Services;
using BaSys.Common.Infrastructure;
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
    public class MetadataKindsController : ControllerBase
    {
        private readonly IBaSysConnectionFactory _connectionFactory;
        private readonly IDbInfoRecordsProvider _dbInfoRecordsProvider;

        public MetadataKindsController(IBaSysConnectionFactory connectionFactory, IDbInfoRecordsProvider dbInfoRecordsProvider)
        {
            _connectionFactory = connectionFactory;
            _dbInfoRecordsProvider = dbInfoRecordsProvider;
        }


        [HttpPost]
        public async Task<IActionResult> CreateItem(MetadataKindSettings settings)
        {
            var dbName = GetDbName();
            var dbInfoRecord = _dbInfoRecordsProvider.GetDbInfoRecordByDbName(dbName);

            var result = new ResultWrapper<int>();

            if (dbInfoRecord == null)
            {
                result.Error(-1, $"Cannot get DbInfoRecord for base {dbName}");
                return Ok(result);
            }

            using (var connection = _connectionFactory.CreateConnection(dbInfoRecord.ConnectionString, dbInfoRecord.DbKind))
            {
                var service = new MetadataKindsService();
                service.SetUp(connection);

                result = await service.InsertSettingsAsync(settings, null);
            }

            return Ok(result);

        }

        private string? GetDbName()
        {
            var authUserDbNameClaim = User.Claims.FirstOrDefault(x => x.Type == "DbName");
            return authUserDbNameClaim?.Value;
        }
    }
}
