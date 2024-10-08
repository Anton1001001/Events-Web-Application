using System.Collections;
using EventsWebApplication.Domain.Entities;

namespace EventsWebApplication.Domain.Repositories;

public interface IEventRepository
{
    IQueryable<Event> GetAllAsync();
    IQueryable<User> GetAllUsersAsync(Guid eventId);
    Task<Event?> GetByIdAsync(Guid eventId, CancellationToken cancellationToken);
    Task<Event?> GetByNameAsync(string name, CancellationToken cancellationToken);
    Task<Event> AddAsync(Event @event, CancellationToken cancellationToken);
    Task<Event> UpdateAsync(Event @event, CancellationToken cancellationToken);
    Task DeleteAsync(Event @event, CancellationToken cancellationToken);
}
