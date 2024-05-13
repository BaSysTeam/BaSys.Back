using BaSys.Common.Infrastructure;
using BaSys.Host.Abstractions;
using BaSys.Host.DAL;
using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DTO;
using BaSys.Host.Infrastructure.Abstractions;
using BaSys.Host.Services;
using BaSys.SuperAdmin.DAL.Abstractions;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace BaSys.Host.Controllers;

[Route("api/admin/v1/[controller]")]
[ApiController]
public class WorkDbController : ControllerBase
{
    private readonly IWorkDbService _workDbService;
    private readonly IDataSourceProvider _dataSourceProvider;
    private readonly IBaSysConnectionFactory _connectionFactory;
    private readonly IDbInitService _dbInitService;

    public WorkDbController(IWorkDbService workDbService,
        IDataSourceProvider dataSourceProvider,
        IBaSysConnectionFactory connectionFactory, 
        IDbInitService dbInitService)
    {
        _workDbService = workDbService;
        _dataSourceProvider = dataSourceProvider;
        _connectionFactory = connectionFactory;
        _dbInitService = dbInitService;
    }

    /// <summary>
    /// Creates new DB. Create required tables and fill neccessary values when db created.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="dto"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    [HttpPost("{id}/initdb")]
    public async Task<IActionResult> InitDb([FromRoute] int id, [FromBody] InitDbRequestDto dto)
    {
        if (string.IsNullOrEmpty(dto.AdminLogin) || string.IsNullOrEmpty(dto.AdminPassword))
            throw new ArgumentException("Admin login and admin password cannot be empty!");

        var result = new ResultWrapper<bool>();

        var dbInfoRecord = _dataSourceProvider.GetConnectionItemByDbInfoId(id);

        if (dbInfoRecord == null)
        {
            result.Error(-1, $"Cannot get DbInfoRecord by id {id}");
            return Ok(result);
        }

        try
        {
            // Initialization by EF Context. Create users, roles etc.
            var state = await _workDbService.InitWorkDb(dto.AdminLogin, dto.AdminPassword, dbInfoRecord.Name);

            // Initialization by Dapper. Create system tables and fill neccessary data when DB created.
            using (var connection = _connectionFactory.CreateConnection(dbInfoRecord.ConnectionString, dbInfoRecord.DbKind))
            {
                _dbInitService.SetUp(connection);
                await _dbInitService.ExecuteAsync();
            }

            result.Success(state);
        }
        catch (Exception ex)
        {
            result.Error(-3, $"Cannot switch activity record.", ex.Message);
        }

        return Ok(result);
    }
}