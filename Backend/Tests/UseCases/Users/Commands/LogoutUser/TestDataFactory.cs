using EventsWebApplication.Application.UseCases.Users.Commands.LogoutUser;
using EventsWebApplication.Domain.Entities;

namespace Tests.UseCases.Users.Commands.LogoutUser;

public static class TestDataFactory
{
    public static RefreshToken CreateRefreshToken()
    {
        return new RefreshToken
        {
            Token = "test"
        };
    }

    public static LogoutUserCommand CreateLogoutUserCommand()
    {
        return new LogoutUserCommand();
    }

    public static User CreateUser()
    {
        return new User
        {
            UserId = Guid.NewGuid(),
            Email = "test"
        };
    }
}