using BaSys.Common.Infrastructure;
using BaSys.Host.Abstractions;
using BaSys.Host.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BaSys.Host.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
[Authorize]
public class UserSettingsController : ControllerBase
{
    private readonly IUserSettingsService _userSettingsService;
    
    public UserSettingsController(IUserSettingsService userSettingsService)
    {
        _userSettingsService = userSettingsService;
    }
    
    /// <summary>
    /// Get UserSettings for authorized user
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        try
        {
            var userSettingsResult = await _userSettingsService.GetUserSettings();
            return Ok(userSettingsResult);
        }
        catch (Exception e)
        {
            var result = new ResultWrapper<UserSettingsDto>();
            result.Error(-1, $"Error: {e}");
            
            return Ok(result);
        }
    }
}