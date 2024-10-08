using EventsWebApplication.Application.UseCases.Users.Queries.GetEventUsers;
using EventsWebApplication.Domain.Entities;

namespace Tests.UseCases.Users.Queries.GetEventUsers;

public static class TestDataFactory
{
    public static GetEventUsersQuery CreateGetEventUsersQuery(
        Guid? eventId = null,
        int pageNumber = 1,
        int pageSize = 5)
    {
        return new GetEventUsersQuery
        {
            EventId = eventId ?? Guid.NewGuid(),
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public static Event CreateEvent(GetEventUsersQuery request)
    {
        return new Event
        {
            EventId = request.EventId,
            Name = "test"
        };
    }

    public static User CreateUser(Event @event, Guid? userId = null)
    {
        return new User
        {
            UserId = userId ?? Guid.NewGuid(),
            Events = [@event],
            EventUsers = [new EventUser
            {
                EventId = @event.EventId, 
                RegistrationDate = DateTime.Now
            }]
        };
    }

    public static IQueryable<User> CreateUsersQuery(Event @event, int count = 5)
    {
        return Enumerable.Repeat(CreateUser(@event), count).AsQueryable();
    }
}