using IdentityService.Models;

namespace IdentityService.Services;

public interface ITokenService
{
    //string GenerateToken(User user);
    //bool ValidateToken(string token);
    //Guid GetUserIdFromToken(string token);
    //string GetUserRoleFromToken(string token);
}

public class JwtTokenService : ITokenService
{
    private readonly IConfiguration _configuration;

    public JwtTokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateToken(Identity user)
    {
        return "";
        // Реализация генерации JWT
    }

    // Другие методы...
}