using EventsWebApplication.Domain.Entities;

namespace EventsWebApplication.Application.Services.Interfaces;

public interface IEventService
{
    Task<(IEnumerable<Event>, int totalCount)> GetAllEventsAsync(
        int page,
        int pageSize,
        string? title,
        DateTime? date,
        string? category,
        string? location);
    Task<ICollection<User>> GetEventUsersAsync(Guid eventId);
    Task<Event?> GetEventByIdAsync(Guid id);
    Task<Event?> GetEventByNameAsync(string name);
    Task<Guid> AddEventAsync(Event @event);
    Task<Guid> UpdateEventAsync(Event @event);
    Task<Guid> DeleteEventAsync(Guid id);
}