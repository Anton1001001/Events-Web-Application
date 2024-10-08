namespace EventsWebApplication.Application.Errors.Base;

public class InternalServerError : BaseError
{
    public InternalServerError(string code, string message) : base(code, message)
    {
    }
}