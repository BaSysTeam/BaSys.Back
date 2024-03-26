using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BaSys.Host.Identity;
using BaSys.Host.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace BaSys.Host.Infrastructure.JwtAuth;

public class JwtAuthService : IJwtAuthService
{
    private readonly SignInManager<WorkDbUser> _signInManager;
    private readonly UserManager<WorkDbUser> _userManager;
    private readonly IConfiguration _configuration;
    
    public JwtAuthService(SignInManager<WorkDbUser> signInManager,
        UserManager<WorkDbUser> userManager,
        IConfiguration configuration)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _configuration = configuration;
    }
    
    public async Task<string?> GenerateToken(string login, string password)
    {
        if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            return null;
        
        var user = await _userManager.FindByEmailAsync(login);
        if (user == null)
            return null;
        
        var result = await _signInManager.CheckPasswordSignInAsync(user, password, false);
        if (!result.Succeeded)
            return null;
        
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, login),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var tokenKey = _configuration["Jwt:TokenKey"];
        if (string.IsNullOrEmpty(tokenKey))
            return null;

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        if (!int.TryParse(_configuration["Jwt:JwtExpiresMinutes"], out var jwtExpiresMinutes))
            jwtExpiresMinutes = 60;

        var token = new JwtSecurityTokenHandler()
            .WriteToken(new JwtSecurityToken("Issuer",
                "Issuer",
                claims,
                expires: DateTime.Now.AddMinutes(jwtExpiresMinutes),
                signingCredentials: creds));
        
        return token;
    }
}