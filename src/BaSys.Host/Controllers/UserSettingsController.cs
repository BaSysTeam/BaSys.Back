using System.Security.Claims;
using BaSys.Common.DTO;
using BaSys.Common.Infrastructure;
using BaSys.Host.Abstractions;
using BaSys.Host.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BaSys.Host.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
// [Authorize]
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

    /// <summary>
    /// Update user settings
    /// </summary>
    /// <param name="userSettings"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> Update([FromBody] UserSettingsDto userSettings)
    {
        try
        {
            var result = await _userSettingsService.UpdateUserSettings(userSettings);
            return Ok(result);
        }
        catch (Exception e)
        {
            var result = new ResultWrapper<bool>();
            result.Error(-1, $"Error: {e}");
            
            return Ok(result);
        }
    }
    
    /// <summary>
    /// Get available languages
    /// </summary>
    /// <returns></returns>
    [HttpGet("GetLanguages")]
    public IActionResult GetLanguages()
    {
        try
        {
            var languages = _userSettingsService.GetLanguages();
            return Ok(languages);
        }
        catch (Exception e)
        {
            var result = new ResultWrapper<List<EnumValuesDto>>();
            result.Error(-1, $"Error: {e}");
            
            return Ok(result);
        }
    }

    /// <summary>
    /// Change password for current user
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpPost("ChangePassword")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
    {
        try
        {
            var userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            var result = await _userSettingsService.ChangePassword(userId, dto.OldPassword, dto.NewPassword);
            return Ok(result);
        }
        catch (Exception e)
        {
            var result = new ResultWrapper<List<EnumValuesDto>>();
            result.Error(-1, $"Error: {e}");
            
            return Ok(result);
        }
    }
}