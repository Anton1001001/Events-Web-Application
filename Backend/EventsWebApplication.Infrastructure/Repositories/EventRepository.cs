
using EventsWebApplication.Application.UseCases.Events.Queries.GetUserEvents;
using EventsWebApplication.Domain.Entities;
using EventsWebApplication.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace EventsWebApplication.Infrastructure.Repositories;

public class EventRepository : IEventRepository
{
    private readonly EventsWebApplicationDbContext _context;
    public EventRepository(EventsWebApplicationDbContext context)
    {
        _context = context;
    }

    public IQueryable<Event> GetAllAsync()
    {
        return _context.Events
            .AsNoTracking()
            .OrderBy(e => e.EventId)
            .AsQueryable();
    }

    
    public IQueryable<User> GetAllUsersAsync(Guid eventId)
    {
        return _context.EventUsers
            .Where(eu => eu.EventId == eventId)
            .Select(eu => eu.User)
            .OrderBy(u => u.UserId);
    }


    public async Task<Event?> GetByIdAsync(Guid eventId, CancellationToken cancellationToken = default)
    {
        var @event = await _context.Events
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.EventId == eventId, cancellationToken: cancellationToken);
        
        return @event;
    }

    public async Task<Event> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var @event = await _context.Events
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Name.ToLower() == name.ToLower(), cancellationToken: cancellationToken);
        return @event;
    }
    
    public async Task<Event> AddAsync(Event @event, CancellationToken cancellationToken = default)
    {
        await _context.Events.AddAsync(@event, cancellationToken);
        return @event;
    }
    

    public async Task<Event> UpdateAsync(Event @event, CancellationToken cancellationToken = default)
    {
        _context.Events.Update(@event);
        return await Task.FromResult(@event);
    }

    
    public async Task DeleteAsync(Event @event, CancellationToken cancellationToken = default)
    {
        _context.Events.Remove(@event);
        await Task.CompletedTask;
    }
}