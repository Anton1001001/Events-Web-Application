using EventsWebApplication.Application.UseCases.Users.Commands.RegisterUser;
using EventsWebApplication.Domain.Entities;

namespace Tests.UseCases.Users.Commands.RegisterUser;

public static class TestDataFactory
{
    public static RegisterUserCommand CreateRegisterUserCommand(
        string email = "test",
        string password = "test")
    {
        return new RegisterUserCommand
        {
            Email = email,
            Password = password
        };
    }

    public static User CreateUser(RegisterUserCommand request)
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
}