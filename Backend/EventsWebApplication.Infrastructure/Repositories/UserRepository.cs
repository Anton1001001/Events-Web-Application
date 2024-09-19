using AutoMapper;
using EventsWebApplication.Application.DTOs;
using EventsWebApplication.Application.Repositories;
using EventsWebApplication.Domain.Entities;
using EventsWebApplication.Domain.Enums;
using EventsWebApplication.Infrastructure.DbEntities;
using Microsoft.EntityFrameworkCore;

namespace EventsWebApplication.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    readonly EventsWebApplicationDbContext _context;
    readonly IMapper _mapper;

    public UserRepository(EventsWebApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Guid> AddAsync(User user)
    {
        var userEntity = _mapper.Map<UserEntity>(user);
        userEntity.Role = Role.User;
        await _context.Users.AddAsync(userEntity);
        return userEntity.Id;
        
    }
    
    public async Task<User> GetByEmailAsync(string email)
    {
        var userEntity = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Email == email);
        return _mapper.Map<User>(userEntity);
    }

    public async Task<User> GetByRefreshTokenAsync(string refreshToken)
    {
        var userEntity = await _context.Users
            .AsNoTracking()
            .Include(p => p.RefreshTokens)
            .FirstOrDefaultAsync(p => p.RefreshTokens.Any(rt => rt.Token == refreshToken));

        return _mapper.Map<User>(userEntity);
    }

    public async Task<bool> RegisterInEventAsync(Guid userId, Guid eventId)
    {
        var exists = await _context.EventUsers
            .AnyAsync(eu => eu.UserId == userId && eu.EventId == eventId);
        
        if (!exists)
        {
            _context.EventUsers.Add(new EventUserEntity { EventId = eventId, UserId = userId });
        }

        return exists;
    }

    public async Task<bool> IsRegisteredForEventAsync(Guid userId, Guid eventId)
    {
        return await _context.EventUsers.AnyAsync(eu => eu.EventId == eventId && eu.UserId == userId);
    }

    public async Task<User> GetUserByIdAsync(Guid id)
    {
        var userEntity = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id);
        var user = _mapper.Map<User>(userEntity);
        return user;
    }

    public async Task<ICollection<Event>> GetEventsAsync(Guid userId)
    {
        var userEntities = await _context.Users
            .AsNoTracking()
            .Include(u => u.Events)
            .FirstOrDefaultAsync(u => u.Id == userId);
        
        var eventEntities = userEntities.Events;
        var @events = _mapper.Map<ICollection<Event>>(eventEntities);
        return @events;
    }

    public async Task<Guid> CancelRegistrationForEventAsync(Guid userId, Guid eventId)
    {
        
        var registration = await _context.EventUsers
            .FirstOrDefaultAsync(eu => eu.EventId == eventId && eu.UserId == userId);
        
        _context.EventUsers.Remove(registration);

        return userId;

    }
    

    public async Task AddRefreshTokenAsync(Guid userId, RefreshTokenDto refreshToken)
    {
        var userEntity = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        var refreshTokenEntity = _mapper.Map<RefreshTokenEntity>(refreshToken);
        userEntity.RefreshTokens.Add(refreshTokenEntity);
    }

    public async Task RemoveRefreshTokenAsync(Guid userId, string refreshToken)
    {
        var userEntity = await _context.Users
            .Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (userEntity == null)
        {
            throw new Exception("User not found");
        }

        var tokenEntity = userEntity.RefreshTokens
            .FirstOrDefault(rt => rt.Token == refreshToken);

        if (tokenEntity == null)
        {
            throw new Exception("Refresh token not found");
        }

        userEntity.RefreshTokens.Remove(tokenEntity);

    }

    public async Task RemoveExpiredRefreshTokensAsync(Guid userId)
    {
        var userEntity = await _context.Users
            .Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (userEntity == null)
        {
            throw new Exception("User not found");
        }
        
        var expiredTokens = userEntity.RefreshTokens
            .Where(rt => rt.Expires <= DateTime.UtcNow)
            .ToList();

        foreach (var token in expiredTokens)
        {
            userEntity.RefreshTokens.Remove(token);
        }
    }

    public async Task<Guid> UpdateAsync(User user)
    {
        var userEntity = _mapper.Map<UserEntity>(user);
        _context.Users.Update(userEntity);
        return user.Id;
    }
}