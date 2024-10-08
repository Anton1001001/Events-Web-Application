using EventsWebApplication.Application.UseCases.Events.Queries.GetEventById;
using EventsWebApplication.Domain.Entities;

namespace Tests.UseCases.Events.Queries.GetEventById;

public static class TestDataFactory
{
    public static GetEventByIdQuery CreateGetEventByIdQuery(
        Guid? eventId = null)
    {
        return new GetEventByIdQuery
        {
            EventId = eventId ?? Guid.NewGuid(),
        };
    }

    public static Event CreateEvent(GetEventByIdQuery query)
    {
        return new Event
        {
            EventId = query.EventId,
            Name = "test"
        };
    }

    public static EventResponse CreateEventResponse(Event @event)
    {
        return new EventResponse
        {
            EventId = @event.EventId,
            Name = @event.Name,
        };
    }

    public static IQueryable<User> CreateUsersQuery(int seatsOccupied = 5)
    {
        return Enumerable.Repeat(new User(), seatsOccupied).AsQueryable();
    }
}