namespace EventsWebApplication.Application.Errors.Base;

public class ConflictError : BaseError
{
    public ConflictError(string code, string message) : base(code, message)
    {
    }
}