namespace BaSys.Host.Infrastructure.JwtAuth
{
    public interface IJwtAuthService
    {
        Task<string> GenerateToken(string login, string password);
    }
}