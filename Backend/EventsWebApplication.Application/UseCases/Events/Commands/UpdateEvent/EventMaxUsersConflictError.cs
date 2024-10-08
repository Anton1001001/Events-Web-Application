using EventsWebApplication.Application.Errors.Base;

namespace EventsWebApplication.Application.UseCases.Events.Commands.UpdateEvent;

public class EventMaxUsersConflictError : ConflictError
{
    public EventMaxUsersConflictError(string code = "EventMaxUsers.Conflict", 
        string message = "The new value of maximum number of users cannot be less than the number of already occupied seats.") 
        : base(code, message)
    {
    }
}