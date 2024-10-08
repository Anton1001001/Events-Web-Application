using EventsWebApplication.Application.UseCases.Events.Queries.GetAllEvents;
using EventsWebApplication.Domain.Entities;
using EventResponse = EventsWebApplication.Application.UseCases.Events.Commands.UpdateEvent.EventResponse;

namespace Tests.UseCases.Events.Queries.GetAllEvents;

public static class TestDataFactory
{
    public static GetAllEventsQuery CreateGetAllEventsQuery(
        int pageNumber = 1,
        int pageSize = 5)
    {
        return new GetAllEventsQuery
        {
            Page = pageNumber,
            PageSize = pageSize
        };
    }

    private static Event CreateEvent(
        string name = "test",
        int maxUsers = 20)
    {
        return new Event
        {
            Name = name,
            MaxUsers = maxUsers,
            Users = []
        };
    }

    public static IQueryable<Event> CreateEventsQuery(int count = 5)
    {
        return Enumerable.Repeat(CreateEvent(), count).AsQueryable();
    }
}