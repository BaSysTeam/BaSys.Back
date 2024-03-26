using BaSys.Common.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using BaSys.SuperAdmin.DAL.Providers;
using Microsoft.Data.SqlClient;
using Npgsql;
using System.Data;
using BaSys.Common.Enums;
using Microsoft.AspNetCore.Connections;
using BaSys.Host.DAL;
using BaSys.Host.DAL.TableManagers;
using BaSys.SuperAdmin.DAL.Models;
using Microsoft.AspNetCore.Http.HttpResults;

namespace BaSys.Constructor.Controllers
{
    [Route("api/constructor/v1/[controller]")]
    [ApiController]
    public class ConstructorTestController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly DbInfoRecordsProvider _dbInfoRecordsProvider;

        //public ConstructorTestController(IConfiguration configuration, DbInfoRecordsProvider dbInfoRecordsProvider)
        //{
        //    _configuration = configuration;
        //    _dbInfoRecordsProvider = dbInfoRecordsProvider; 
        //}

        public ConstructorTestController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("Ping")]
        public IActionResult Ping()
        {
            var result = new ResultWrapper<DateTime>();
            result.Success(DateTime.Now);

            return Ok(result);
        }

        [HttpPost("CreateMetadataGroupTable")]
        public async Task<IActionResult> CreateMetadataGroupTable()
        {
            //var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = new ResultWrapper<int>();
            var dbName = User.Claims.FirstOrDefault(c => c.Type == "DbName")?.Value;

            if (string.IsNullOrEmpty(dbName))
            {
                result.Error(-1, $"Cannot get dbName.");
                return Ok(result);
            }

            // TODO: Get connection strings via service.
            //var dbInfoRecord = _dbInfoRecordsProvider.GetDbInfoRecordByDbName(dbName);

            //if ( dbInfoRecord == null )
            //{
            //    result.Error(-1, $"Cannot get DbInfoRecord.");
            //    return Ok(result);
            //}

            var dbInfoRecord = GetDbInfoRecordTmp();

            var factory = new ConnectionFactory();
            using (IDbConnection connection = factory.CreateConnection(dbInfoRecord.ConnectionString, dbInfoRecord.DbKind))
            {
                var tableManager = new MetadataGroupManager(connection);

                try
                {
                    await tableManager.CreateTableAsync(null);
                    result.Success(1, $"Metadata group table created");
                }
                catch (Exception ex)
                {
                    result.Error(-1, ex.Message);
                }
            }



            return Ok(result);
        }

        [HttpPost("DeleteMetadataGroupTable")]
        public async Task<IActionResult> DeleteMetadataGroupTable()
        {
            var result = new ResultWrapper<int>();
            var dbInfoRecord = GetDbInfoRecordTmp();

            var factory = new ConnectionFactory();
            using (IDbConnection connection = factory.CreateConnection(dbInfoRecord.ConnectionString, dbInfoRecord.DbKind))
            {
                var tableManager = new MetadataGroupManager(connection);

                try
                {
                    await tableManager.DropTableAsync(null);
                    result.Success(1, $"Metadata group table dropped");
                }
                catch (Exception ex)
                {
                    result.Error(0, ex.Message);
                }
            }

            return Ok(result);
        }

        private DbInfoRecord GetDbInfoRecordTmp()
        {
            var connectionString = _configuration.GetSection("InitAppSettings:MainDb:ConnectionString").Value;
            var dbKindStr = _configuration.GetSection("InitAppSettings:MainDb:DbKind").Value;

            if (string.IsNullOrEmpty(connectionString))
            {
                return null;
            }

            var dbKindInt = int.Parse(dbKindStr);
            var dbKind = (DbKinds)dbKindInt;

            var dbInfoRecord = new DbInfoRecord()
            {
                DbKind = dbKind,
                ConnectionString = connectionString
            };

            return dbInfoRecord;
        }
    }
}
