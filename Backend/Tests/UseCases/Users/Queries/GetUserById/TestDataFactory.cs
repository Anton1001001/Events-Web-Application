using EventsWebApplication.Application.UseCases.Users.Queries.GetUserById;
using EventsWebApplication.Domain.Entities;

namespace Tests.UseCases.Users.Queries.GetUserById;

public static class TestDataFactory
{
    public static GetUserByIdQuery CreateGetUserByIdQuery(Guid? userId = null)
    {
        return new GetUserByIdQuery
        {
            UserId = userId ?? Guid.NewGuid()
        };
    }

    public static User CreateUser(GetUserByIdQuery request)
    {
        return new User
        {
            UserId = request.UserId,
            Email = "test"
        };
    }

    public static UserResponse CreateUserResponse(User user)
    {
        return new UserResponse
        {
            UserId = user.UserId,
            Email = user.Email
        };
    }
}