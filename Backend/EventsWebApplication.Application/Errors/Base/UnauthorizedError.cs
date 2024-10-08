namespace EventsWebApplication.Application.Errors.Base;

public class UnauthorizedError : BaseError
{
    public UnauthorizedError(string code, string message) : base(code, message)
    {
    }
}