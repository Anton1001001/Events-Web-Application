using EventsWebApplication.Application.Errors.Base;

namespace EventsWebApplication.Application.Errors;

public class UserNotFoundError : NotFoundError
{
    public UserNotFoundError(string code = "User.NotFound", string message = "User not found") : base(code, message)
    {
    }
}