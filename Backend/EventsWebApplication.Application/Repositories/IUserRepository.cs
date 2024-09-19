using EventsWebApplication.Application.DTOs;
using EventsWebApplication.Domain.Entities;
using EventsWebApplication.Domain.Enums;

namespace EventsWebApplication.Application.Repositories;

public interface IUserRepository
{
    Task<Guid> AddAsync(User user);
    Task<User> GetByEmailAsync(string email);
    Task<User> GetByRefreshTokenAsync(string refreshToken);
    Task<bool> RegisterInEventAsync(Guid userId, Guid eventId);
    Task<bool> IsRegisteredForEventAsync(Guid userId, Guid eventId);
    Task<User> GetUserByIdAsync(Guid id);
    Task<ICollection<Event>> GetEventsAsync(Guid id);
    Task<Guid> CancelRegistrationForEventAsync(Guid userId, Guid eventId);
    Task AddRefreshTokenAsync(Guid userId, RefreshTokenDto refreshToken);
    Task RemoveRefreshTokenAsync(Guid userId, string refreshToken);
    Task RemoveExpiredRefreshTokensAsync(Guid userId);
    Task<Guid> UpdateAsync(User user);
}