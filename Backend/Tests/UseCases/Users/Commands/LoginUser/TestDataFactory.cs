using EventsWebApplication.Application.UseCases.Users.Commands.LoginUser;
using EventsWebApplication.Domain.Entities;
using UserResponse = EventsWebApplication.Application.UseCases.Users.Commands.LoginUser.UserResponse;

namespace Tests.UseCases.Users.Commands.LoginUser;

public static class TestDataFactory
{
    public static LoginUserCommand CreateLoginUserCommand(
        string email = "test",
        string password = "test")
    {
        return new LoginUserCommand
        {
            Email = email,
            Password = password
        };
    }

    public static User CreateUser(LoginUserCommand request)
    {
        return new User
        {
            UserId = Guid.NewGuid(),
            Email = request.Email,
        };
    }

    public static UserResponse CreateUserResponse(User user)
    {
        return new UserResponse
        {
            UserId = user.UserId,
            Email = user.Email,
        };
    }

    public static RefreshToken CreateRefreshToken()
    {
        return new RefreshToken
        {
            Token = "test"
        };
    }
    
    
}