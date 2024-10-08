using EventsWebApplication.Domain.Entities;

namespace EventsWebApplication.Application.Helpers;

public interface IJwtProvider
{
    string GenerateJwtToken(User user);
    string GetUserIdFromToken(string token);
    bool VerifyPassword(string password, string passwordHash);
    RefreshToken GenerateRefreshToken();
}