using EventsWebApplication.Domain.Entities;

namespace EventsWebApplication.Domain.Repositories;

public interface IUserRepository
{
    Task<User> AddAsync(User user, CancellationToken cancellationToken);
    Task<User> GetByEmailAsync(string email, CancellationToken cancellationToken);
    Task<User> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken);
    Task RegisterForEventAsync(Guid userId, Guid eventId, CancellationToken cancellationToken);
    Task<bool> IsRegisteredForEventAsync(Guid userId, Guid eventId);
    Task<User> GetByIdAsync(Guid userId, CancellationToken cancellationToken);
    IQueryable<Event> GetEvents(Guid id);
    Task<bool> CancelRegistrationForEventAsync(Guid userId, Guid eventId, CancellationToken cancellationToken);
    Task AddRefreshTokenAsync(Guid userId, RefreshToken refreshToken, CancellationToken cancellationToken);
    Task RemoveRefreshTokenAsync(Guid userId, string refreshToken, CancellationToken cancellationToken);
    Task RemoveExpiredRefreshTokensAsync(Guid userId, CancellationToken cancellationToken);
    Task<User> UpdateAsync(User user, CancellationToken cancellationToken);
}