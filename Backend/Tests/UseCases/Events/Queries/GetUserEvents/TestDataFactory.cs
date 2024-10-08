using EventsWebApplication.Application.UseCases.Events.Queries.GetUserEvents;
using EventsWebApplication.Domain.Entities;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Tests.UseCases.Events.Queries.GetUserEvents;

public static class TestDataFactory
{
    public static GetUserEventsQuery CreateGetUserEventsQuery(
        Guid? userId = null,
        int pageNumber = 1,
        int pageSize = 5)
    {
        return new GetUserEventsQuery
        {
            UserId = userId ?? Guid.NewGuid(),
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public static IQueryable<Event> CreateEventsQuery(int count = 5)
    {
        return Enumerable.Repeat(CreateEvent(), count).AsQueryable();
    }

    public static User CreateUser(GetUserEventsQuery request)
    {
        return new User
        {
            UserId = request.UserId,
            Email = "test"
        };
    }

    public static Event CreateEvent(
        Guid? eventId = null,
        int maxUsers = 10)
    {
        return new Event
        {
            EventId = eventId ?? Guid.NewGuid(),
            MaxUsers = maxUsers,
            Users = []
        };
    }
    
}