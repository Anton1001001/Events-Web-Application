
using EventsWebApplication.Domain.Entities;
using EventsWebApplication.Domain.Enums;
using EventsWebApplication.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace EventsWebApplication.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    readonly EventsWebApplicationDbContext _context;

    public UserRepository(EventsWebApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<User> AddAsync(User user , CancellationToken cancellationToken = default)
    {
        await _context.Users.AddAsync(user, cancellationToken);
        return user;

    }
    
    public async Task<User> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Email == email, cancellationToken);
        return user;
    }

    public async Task<User> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        var userEntity = await _context.Users
            .AsNoTracking()
            .Include(p => p.RefreshTokens)
            .FirstOrDefaultAsync(p => p.RefreshTokens.Any(rt => rt.Token == refreshToken), cancellationToken);

        return userEntity;
    }

    public async Task RegisterForEventAsync(Guid userId, Guid eventId, CancellationToken cancellationToken = default)
    {
        await _context.EventUsers.AddAsync(
            new EventUser
            {
                EventId = eventId, 
                UserId = userId,
                RegistrationDate = DateTime.Now
            }, cancellationToken);
    }

    public async Task<bool> IsRegisteredForEventAsync(Guid userId, Guid eventId)
    {
        return await _context.EventUsers.AnyAsync(eu => eu.EventId == eventId && eu.UserId == userId);
    }

    public async Task<User> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.UserId == userId, cancellationToken);
        return user;
    }

    public IQueryable<Event> GetEvents(Guid userId)
    {
        return _context.EventUsers
            .Where(eu => eu.UserId == userId)
            .Select(eu => eu.Event)
            .OrderBy(e => e.EventId);

    }

    public async Task<bool> CancelRegistrationForEventAsync(Guid userId, Guid eventId, CancellationToken cancellationToken = default)
    {
        var eventUser = await _context.EventUsers
            .FirstOrDefaultAsync(eu => eu.EventId == eventId && eu.UserId == userId, cancellationToken);

        if (eventUser is null)
        {
            return false;
        }
        
        _context.EventUsers.Remove(eventUser);

        return true;
    }
    
    public async Task AddRefreshTokenAsync(Guid userId, RefreshToken refreshToken, CancellationToken cancellationToken = default)
    {
        var userEntity = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId, cancellationToken);
        userEntity.RefreshTokens.Add(refreshToken);
    }

    public async Task RemoveRefreshTokenAsync(Guid userId, string refreshToken, CancellationToken cancellationToken = default)
    {
        var userEntity = await _context.Users
            .Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(u => u.UserId == userId, cancellationToken);
            

        var tokenEntity = userEntity.RefreshTokens
            .FirstOrDefault(rt => rt.Token == refreshToken);

        userEntity.RefreshTokens.Remove(tokenEntity);

    }

    public async Task RemoveExpiredRefreshTokensAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var userEntity = await _context.Users
            .Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(u => u.UserId == userId, cancellationToken);
        
        var expiredTokens = userEntity.RefreshTokens
            .Where(rt => rt.Expires <= DateTime.UtcNow)
            .ToList();

        foreach (var token in expiredTokens)
        {
            userEntity.RefreshTokens.Remove(token);
        }
    }

    public async Task<User> UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        _context.Users.Update(user);
        return await Task.FromResult(user);
    }
}