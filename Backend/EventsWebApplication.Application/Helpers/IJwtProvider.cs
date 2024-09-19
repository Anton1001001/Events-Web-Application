using EventsWebApplication.Application.DTOs;
using EventsWebApplication.Domain.Entities;

namespace EventsWebApplication.Application.Helpers;

public interface IJwtProvider
{
    string GenerateJwtToken(User user);
    RefreshTokenDto GenerateRefreshToken();
}