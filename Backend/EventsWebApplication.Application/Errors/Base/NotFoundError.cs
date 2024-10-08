namespace EventsWebApplication.Application.Errors.Base;

public class NotFoundError : BaseError
{
    public NotFoundError(string code, string message) : base(code, message)
    {
    }
}