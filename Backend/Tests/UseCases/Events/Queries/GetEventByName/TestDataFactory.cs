using EventsWebApplication.Application.UseCases.Events.Queries.GetEventByName;
using EventsWebApplication.Domain.Entities;
using EventResponse = EventsWebApplication.Application.UseCases.Events.Queries.GetEventByName.EventResponse;

namespace Tests.UseCases.Events.Queries.GetEventByName;

public static class TestDataFactory
{
    public static GetEventByNameQuery CreateGetEventByNameQuery(
        string name = "test")
    {
        return new GetEventByNameQuery
        {
            Name = name
        };
    }

    public static Event CreateEvent(GetEventByNameQuery query)
    {
        return new Event
        {
            Name = "test"
        };
    }

    public static EventResponse CreateEventResponse(Event @event)
    {
        return new EventResponse
        {
            Name = @event.Name,
        };
    }

    public static IQueryable<User> CreateUsersQuery(int seatsOccupied = 5)
    {
        return Enumerable.Repeat(new User(), seatsOccupied).AsQueryable();
    }
}