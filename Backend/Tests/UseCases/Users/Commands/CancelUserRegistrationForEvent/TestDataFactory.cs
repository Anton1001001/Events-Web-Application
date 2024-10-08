using EventsWebApplication.Application.UseCases.Users.Commands.CancelUserRegistrationForEvent;
using EventsWebApplication.Domain.Entities;

namespace Tests.UseCases.Users.Commands.CancelUserRegistrationForEvent;

public static class TestDataFactory
{
    public static CancelUserRegistrationForEventCommand CreateCancelUserRegistrationForEventCommand(
        Guid? userId = null,
        Guid? eventId = null)
    {
        return new CancelUserRegistrationForEventCommand
        {
            UserId = userId ?? Guid.NewGuid(),
            EventId = eventId ?? Guid.NewGuid()
        };
    }

    public static User CreateUser(CancelUserRegistrationForEventCommand request)
    {
        return new User
        {
            UserId = request.UserId,
            Email = "test",
        };
    }

    public static Event CreateEvent(CancelUserRegistrationForEventCommand request)
    {
        return new Event
        {
            EventId = request.EventId,
            Name = "test",
        };
    }
}
