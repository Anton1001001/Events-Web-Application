using EventsWebApplication.Application.UseCases.Users.Commands.UpdateUserRefreshToken;
using EventsWebApplication.Domain.Entities;

namespace Tests.UseCases.Users.Commands.UpdateUserRefreshToken;

public static class TestDataFactory
{
    public static UpdateUserRefreshTokenCommand CreateUpdateUserRefreshTokenCommand()
    {
        return new UpdateUserRefreshTokenCommand();
    }

    public static User CreateUser()
    {
        return new User
        {
            UserId = Guid.NewGuid(),
            Email = "test",
        };
    }

    public static RefreshToken CreateRefreshToken()
    {
        return new RefreshToken
        {
            Token = "test",
        };
    }
}