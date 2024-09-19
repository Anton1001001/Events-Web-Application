using EventsWebApplication.Domain.Entities;

namespace EventsWebApplication.Application.Repositories;

public interface IEventRepository
{
    Task<(IEnumerable<Event>, int totalCount)> GetAllAsync(
        int page,
        int pageSize,
        string? title,
        DateTime? date,
        string? category,
        string? location);
    Task<ICollection<User>> GetUsersAsync(Guid eventId);
    Task<Event?> GetByIdAsync(Guid id);
    Task<Event?> GetByNameAsync(string name);
    Task<Guid> AddAsync(Event @event);
    Task<Guid> UpdateAsync(Event @event);
    Task<Guid> DeleteAsync(Guid id);
}
