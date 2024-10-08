using EventsWebApplication.Application.Helpers;

namespace EventsWebApplication.Infrastructure.Authorization;

public class PasswordHasher : IPasswordHasher
{
    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }
}