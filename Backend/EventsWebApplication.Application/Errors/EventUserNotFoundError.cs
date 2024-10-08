using EventsWebApplication.Application.Errors.Base;

namespace EventsWebApplication.Application.Errors;

public class EventUserNotFoundError : NotFoundError
{
    public EventUserNotFoundError(string code = "EventUser.NotFound", string message = "EventUser not found") : base(code, message)
    {
    }
}