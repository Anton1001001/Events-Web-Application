using EventsWebApplication.Application.Errors.Base;

namespace EventsWebApplication.Application.Errors;

public class EventNotFoundError : NotFoundError
{
    public EventNotFoundError(string code = "Event.NotFound", string message = "Event not found") : base(code, message)
    {
    }
}