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
using Microsoft.AspNetCore.Authorization;
using BaSys.SuperAdmin.DAL.Abstractions;
using BaSys.Host.DAL.DataProviders;
using BaSys.Metadata.Models;
using BaSys.Metadata.DTOs;

namespace BaSys.Constructor.Controllers
{
    [Route("api/constructor/v1/[controller]")]
    [ApiController]
    [Authorize]
    public class ConstructorTestController : ControllerBase
    {
        private readonly IDbInfoRecordsProvider _dbInfoRecordsProvider;

        public ConstructorTestController(IDbInfoRecordsProvider dbInfoRecordsProvider)
        {
            _dbInfoRecordsProvider = dbInfoRecordsProvider;
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
            var dbInfoRecord = GetDbInfoRecord();
            //var dbName = User.Claims.FirstOrDefault(c => c.Type == "DbName")?.Value;

            //if (string.IsNullOrEmpty(dbName))
            //{
            //    result.Error(-1, $"Cannot get dbName.");
            //    return Ok(result);
            //}

            //var dbInfoRecord = _dbInfoRecordsProvider.GetDbInfoRecordByDbName(dbName);

            //if (dbInfoRecord == null)
            //{
            //    result.Error(-1, $"Cannot get DbInfoRecord.");
            //    return Ok(result);
            //}

            // var dbInfoRecord = GetDbInfoRecordTmp();

            var factory = new BaSysConnectionFactory();
            using (IDbConnection connection = factory.CreateConnection(dbInfoRecord.ConnectionString, dbInfoRecord.DbKind))
            {
                var tableManager = new MetadataGroupManager(connection);

                var isTable = await tableManager.TableExistsAsync();
                if(isTable)
                {
                    result.Error(-1, $"Table {tableManager.TableName} already exists");
                    return Ok(result);
                }

                try
                {
                    await tableManager.CreateTableAsync();
                    result.Success(1, $"Table  {tableManager.TableName} created");
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
            var dbInfoRecord = GetDbInfoRecord();

            var factory = new BaSysConnectionFactory();
            using (IDbConnection connection = factory.CreateConnection(dbInfoRecord.ConnectionString, dbInfoRecord.DbKind))
            {
                var tableManager = new MetadataGroupManager(connection);

                try
                {
                    await tableManager.DropTableAsync();
                    result.Success(1, $"Table  {tableManager.TableName}  dropped");
                }
                catch (Exception ex)
                {
                    result.Error(0, ex.Message);
                }
            }

            return Ok(result);
        }

        [HttpPost("InsertMetadataGroup")]
        public async Task<IActionResult> InsertMetadataGroup([FromBody] MetadataGroupDto dto)
        {
            var result = new ResultWrapper<int>();
            var dbInfoRecord = GetDbInfoRecord();

            var factory = new BaSysConnectionFactory();
            using (IDbConnection connection = factory.CreateConnection(dbInfoRecord.ConnectionString, dbInfoRecord.DbKind))
            {
                var provider = new MetadataGroupProvider(connection);

                try
                {
                    var item = new MetadataGroup(dto);
                    var rowsAffected = await provider.InsertAsync(item, null);
                    result.Success(rowsAffected, $"Item inserted");
                }
                catch (Exception ex)
                {
                    result.Error(-1, "Cannot insert item", $"Message: {ex.Message}, Query: {provider.LastQuery}");
                }
            }

            return Ok(result);
        }

        [HttpPost("UpdateMetadataGroup")]
        public async Task<IActionResult> UpdateMetadataGroup([FromBody] MetadataGroupDto dto)
        {
            var result = new ResultWrapper<int>();
            var dbInfoRecord = GetDbInfoRecord();

            var factory = new BaSysConnectionFactory();
            using (IDbConnection connection = factory.CreateConnection(dbInfoRecord.ConnectionString, dbInfoRecord.DbKind))
            {
                var provider = new MetadataGroupProvider(connection);

                try
                {
                    var item = new MetadataGroup(dto);
                    var rowsAffected = await provider.UpdateAsync(item, null);
                    result.Success(rowsAffected, $"Item updated");
                }
                catch (Exception ex)
                {
                    result.Error(-1, "Cannot update item", $"Message: {ex.Message}, Query: {provider.LastQuery}");
                }
            }

            return Ok(result);
        }

        [HttpPost("DeleteMetadataGroup/{uid}")]
        public async Task<IActionResult> DeleteMetadataGroup(Guid uid)
        {
            var result = new ResultWrapper<int>();
            var dbInfoRecord = GetDbInfoRecord();

            var factory = new BaSysConnectionFactory();
            using (IDbConnection connection = factory.CreateConnection(dbInfoRecord.ConnectionString, dbInfoRecord.DbKind))
            {
                var provider = new MetadataGroupProvider(connection);

                try
                {
                    var rowsAffected = await provider.DeleteAsync(uid, null);
                    result.Success(rowsAffected, $"Item deleted");
                }
                catch (Exception ex)
                {
                    result.Error(-1, "Cannot delete item", $"Message: {ex.Message}, Query: {provider.LastQuery}");
                }
            }

            return Ok(result);
        }

        [HttpGet("SelectMetadataGroups")]
        public async Task<IActionResult> SelectMetadataGroups()
        {
            var result = new ResultWrapper<IEnumerable<MetadataGroup>>();
            var dbInfoRecord = GetDbInfoRecord();

            var factory = new BaSysConnectionFactory();
            using (IDbConnection connection = factory.CreateConnection(dbInfoRecord.ConnectionString, dbInfoRecord.DbKind))
            {
                var provider = new MetadataGroupProvider(connection);

                try
                {
                    var collection = await provider.GetCollectionAsync(null);
                    result.Success(collection);
                }
                catch (Exception ex)
                {
                    result.Error(0, $"Message: {ex.Message}, Query: {provider.LastQuery}");
                }
            }

            return Ok(result);
        }

        [HttpGet("SelectMetadataGroupItem")]
        public async Task<IActionResult> SelectMetadataGroupItem([FromQuery] Guid uid)
        {
            var result = new ResultWrapper<MetadataGroup>();
            var dbInfoRecord = GetDbInfoRecord();

            var factory = new BaSysConnectionFactory();
            using (IDbConnection connection = factory.CreateConnection(dbInfoRecord.ConnectionString, dbInfoRecord.DbKind))
            {
                var provider = new MetadataGroupProvider(connection);

                try
                {
                    var item = await provider.GetItemAsync(uid, null);
                    result.Success(item);
                }
                catch (Exception ex)
                {
                    result.Error(0, $"Message: {ex.Message}, Query: {provider.LastQuery}");
                }
            }

            return Ok(result);
        }

        [HttpPost("TruncateMetadataGroupTable")]
        public async Task<IActionResult> TruncateMetadataGroupTable()
        {
            var result = new ResultWrapper<int>();
            var dbInfoRecord = GetDbInfoRecord();
            var factory = new BaSysConnectionFactory();

            using (IDbConnection connection = factory.CreateConnection(dbInfoRecord.ConnectionString, dbInfoRecord.DbKind))
            {
                var tableManager = new MetadataGroupManager(connection);

                var isTable = await tableManager.TableExistsAsync();
                if (!isTable)
                {
                    result.Error(-1, $"Table {tableManager.TableName} doesn't exists");
                    return Ok(result);
                }

                try
                {
                    await tableManager.TruncateTableAsync();
                    result.Success(1, $"Table {tableManager.TableName} truncated");
                }
                catch (Exception ex)
                {
                    result.Error(-1, ex.Message);
                }
            }

            return Ok(result);
        }

        [HttpGet("ColumnExists/{columnName}")]
        public async Task<IActionResult> ColumnExists(string columnName)
        {
            var result = new ResultWrapper<int>();
            var dbInfoRecord = GetDbInfoRecord();
            var factory = new BaSysConnectionFactory();

            using (IDbConnection connection = factory.CreateConnection(dbInfoRecord.ConnectionString, dbInfoRecord.DbKind))
            {
                var tableManager = new MetadataGroupManager(connection);
                var isTableExists = await tableManager.TableExistsAsync();
                if (!isTableExists)
                {
                    result.Error(-1, $"Table {tableManager.TableName} doesn't exists");
                    return Ok(result);
                }

                try
                {
                    var isColumnExists = await tableManager.ColumnExistsAsync(columnName);
                    var msg = $"Column {tableManager.TableName} ";
                    msg += isColumnExists ? "exists" : "doesn't exists";

                    result.Success(1, msg);
                }
                catch (Exception ex)
                {
                    result.Error(-1, ex.Message);
                }
            }

            return Ok(result);
        }

        [HttpPost("CreateAppConstantsRecordTable")]
        public async Task<IActionResult> CreateAppConstantsRecordTable()
        {
            var result = new ResultWrapper<int>();
            var dbInfoRecord = GetDbInfoRecord();
            var factory = new BaSysConnectionFactory();
            
            using (IDbConnection connection = factory.CreateConnection(dbInfoRecord.ConnectionString, dbInfoRecord.DbKind))
            {
                var tableManager = new AppConstantsRecordManager(connection);

                var isTable = await tableManager.TableExistsAsync();
                if (isTable)
                {
                    result.Error(-1, $"Table {tableManager.TableName} already exists");
                    return Ok(result);
                }

                try
                {
                    await tableManager.CreateTableAsync();
                    result.Success(1, $"Table  {tableManager.TableName} created");
                }
                catch (Exception ex)
                {
                    result.Error(-1, ex.Message);
                }
            }

            return Ok(result);
        }

        [HttpPost("TruncateAppConstantsRecordTable")]
        public async Task<IActionResult> TruncateAppConstantsRecordTable()
        {
            var result = new ResultWrapper<int>();
            var dbInfoRecord = GetDbInfoRecord();
            var factory = new BaSysConnectionFactory();

            using (IDbConnection connection = factory.CreateConnection(dbInfoRecord.ConnectionString, dbInfoRecord.DbKind))
            {
                var tableManager = new AppConstantsRecordManager(connection);

                var isTable = await tableManager.TableExistsAsync();
                if (!isTable)
                {
                    result.Error(-1, $"Table {tableManager.TableName} doesn't exists");
                    return Ok(result);
                }

                try
                {
                    await tableManager.TruncateTableAsync();
                    result.Success(1, $"Table {tableManager.TableName} truncated");
                }
                catch (Exception ex)
                {
                    result.Error(-1, ex.Message);
                }
            }

            return Ok(result);
        }

        [HttpPost("DeleteAppConstantsRecordTable")]
        public async Task<IActionResult> DeleteAppConstantsRecordTable()
        {
            var result = new ResultWrapper<int>();
            var dbInfoRecord = GetDbInfoRecord();

            var factory = new BaSysConnectionFactory();
            using (IDbConnection connection = factory.CreateConnection(dbInfoRecord.ConnectionString, dbInfoRecord.DbKind))
            {
                var tableManager = new AppConstantsRecordManager(connection);

                try
                {
                    await tableManager.DropTableAsync();
                    result.Success(1, $"Table  {tableManager.TableName}  dropped");
                }
                catch (Exception ex)
                {
                    result.Error(0, ex.Message);
                }
            }

            return Ok(result);
        }

        private DbInfoRecord GetDbInfoRecord()
        {
            var dbName = User.Claims.FirstOrDefault(c => c.Type == "DbName")?.Value;

            if (string.IsNullOrEmpty(dbName))
            {
                return null;
            }

            var dbInfoRecord = _dbInfoRecordsProvider.GetDbInfoRecordByDbName(dbName);

            if (dbInfoRecord == null)
            {
                return null;
            }

            return dbInfoRecord;
        }
    }
}
