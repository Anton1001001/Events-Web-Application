using EventsWebApplication.Application.UseCases.Events.Commands.UpdateEvent;
using EventsWebApplication.Domain.Entities;

namespace Tests.UseCases.Events.Commands.UpdateEvent;

public static class TestDataFactory
{
    public static UpdateEventCommand CreateUpdateEventCommand(
        Guid? eventId = null,
        int maxUsers = 10)
    {
        return new UpdateEventCommand
        {
            EventId = eventId ?? Guid.NewGuid(),
            MaxUsers = maxUsers
        };
    }

    public static Event CreateEvent(UpdateEventCommand command)
    {
        return new Event
        {
            EventId = command.EventId,
            MaxUsers = command.MaxUsers
        };
    }

    public static EventResponse CreateEventResponse(Event @event)
    {
        return new EventResponse
        {
            EventId = @event.EventId,
            MaxUsers = @event.MaxUsers
        };
    }

    public static IQueryable<User> CreateUsersQuery(int seatsOccupied = 5)
    {
        return Enumerable.Repeat(new User(), seatsOccupied).AsQueryable();
    }
}