using EventsWebApplication.Application.Errors.Base;

namespace EventsWebApplication.Application.UseCases.Users.Commands.RegisterUser;

public class UserEmailConflictError : ConflictError
{
    public UserEmailConflictError(string code = "UserEmail.Conflict",
        string message = "The email value conflicts with the state of the database") : base(code, message)
    {

    }
}