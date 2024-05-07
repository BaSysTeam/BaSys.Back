using BaSys.Host.Infrastructure.JwtAuth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BaSys.PublicAPI.Controllers;

[Route("api/public/v1/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IJwtAuthService _authService;
    public AuthController(IJwtAuthService authService)
    {
        _authService = authService;
    }
    
    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GenerateToken(string login, string password, string dbId)
    {
        if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            return BadRequest("Could not create token - empty login or password");
        
        var token = await _authService.GenerateToken(login, password, dbId);
        
        if (string.IsNullOrEmpty(token))
            return BadRequest("Could not create token - wrong login or password");
        
        return Ok(new
        {
            Token = token
        });
    }
}