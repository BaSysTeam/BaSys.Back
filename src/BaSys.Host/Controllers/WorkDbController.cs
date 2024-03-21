using BaSys.Common.Infrastructure;
using BaSys.Host.Abstractions;
using BaSys.Host.DTO;
using Microsoft.AspNetCore.Mvc;

namespace BaSys.Host.Controllers;

[Route("api/admin/v1/[controller]")]
[ApiController]
public class WorkDbController : ControllerBase
{
    private readonly IWorkDbService _workDbService;
    
    public WorkDbController(IWorkDbService workDbService)
    {
        _workDbService = workDbService;
    }
    
    [HttpPost("{id}/initdb")]
    public async Task<IActionResult> InitDb([FromRoute]int id, [FromBody]InitDbRequestDto dto)
    {
        if (string.IsNullOrEmpty(dto.AdminLogin) || string.IsNullOrEmpty(dto.AdminPassword))
            throw new ArgumentException("Admin login and admin password cannot be empty!");
        
        var result = new ResultWrapper<bool>();

        try
        {
            var state = await _workDbService.InitWorkDb(dto.AdminLogin, dto.AdminPassword);
            result.Success(state);
        }
        catch (Exception ex)
        {
            result.Error(-3, $"Cannot switch activity record.", ex.Message);
        }
        
        return Ok(result);
    }
}