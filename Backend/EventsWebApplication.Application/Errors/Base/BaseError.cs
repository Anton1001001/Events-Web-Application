using FluentResults;

namespace EventsWebApplication.Application.Errors.Base;

public abstract class BaseError : Error
{
    public string Code { get; set; }

    public BaseError(string code, string message) : base(message)
    {
        Code = code;
    }
}