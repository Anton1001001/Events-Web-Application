using EventsWebApplication.Application.Errors.Base;

namespace EventsWebApplication.Application.UseCases.Users.Commands.LoginUser;

public class InvalidCredentialsError : UnauthorizedError
{
    public InvalidCredentialsError(string code = "UserCredentials.Invalid", 
        string message = "The provided email or password is incorrect.") : base(code, message)
    {
    }
}