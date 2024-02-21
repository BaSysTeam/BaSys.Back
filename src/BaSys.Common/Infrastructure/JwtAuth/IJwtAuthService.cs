namespace BaSys.Common.Infrastructure.JwtAuth;

public interface IJwtAuthService
{
    Task<string?> GenerateToken(string login, string password);
}