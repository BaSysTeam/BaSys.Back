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
    private readonly IConnectionFactory _connectionFactory;
    private readonly IDbInitService _dbInitService;

    public WorkDbController(IWorkDbService workDbService,
        IDataSourceProvider dataSourceProvider,
        IConnectionFactory connectionFactory, 
        IDbInitService dbInitService)
    {
        _workDbService = workDbService;
        _dataSourceProvider = dataSourceProvider;
        _connectionFactory = connectionFactory;
        _dbInitService = dbInitService;
    }

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
            var state = await _workDbService.InitWorkDb(dto.AdminLogin, dto.AdminPassword);

            using (IDbConnection connection = _connectionFactory.CreateConnection(dbInfoRecord.ConnectionString, dbInfoRecord.DbKind))
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