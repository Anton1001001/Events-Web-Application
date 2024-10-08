using EventsWebApplication.Application.UseCases.Users.Commands.RegisterUserForEvent;
using EventsWebApplication.Domain.Entities;

namespace Tests.UseCases.Users.Commands.RegisterUserForEvent;

public static class TestDataFactory
{
    public static RegisterUserForEventCommand CreateRegisterUserForEventCommand(
        Guid? userId = null,
        Guid? eventId = null)
    {
        return new RegisterUserForEventCommand
        {
            UserId = userId ?? Guid.NewGuid(),
            EventId = eventId ?? Guid.NewGuid()
        };
    }

    public static User CreateUser(RegisterUserForEventCommand request)
    {
        return new User
        {
            UserId = request.UserId,
            Email = "test",
        };
    }

    public static Event CreateEvent(RegisterUserForEventCommand request)
    {
        return new Event
        {
            EventId = request.EventId,
            Name = "test",
        };
    }
    
}