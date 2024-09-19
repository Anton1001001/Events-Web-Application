using System.Collections;
using AutoMapper;
using EventsWebApplication.Application.Repositories;
using EventsWebApplication.Domain.Entities;
using EventsWebApplication.Infrastructure.DbEntities;
using Microsoft.EntityFrameworkCore;

namespace EventsWebApplication.Infrastructure.Repositories;

public class EventRepository : IEventRepository
{
    private readonly EventsWebApplicationDbContext _context;
    private readonly IMapper _mapper;

    public EventRepository(EventsWebApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<(IEnumerable<Event>, int totalCount)> GetAllAsync(
        int page, 
        int pageSize, 
        string? title = null, 
        DateTime? date = null, 
        string? category = null, 
        string? location = null)
    {
        var query = _context.Events
            .Include(e => e.Users)
            .AsNoTracking();

        if (!string.IsNullOrEmpty(title))
        {
            query = query.Where(e => e.Name.Contains(title));
        }

        if (date.HasValue)
        {
            query = query.Where(e => e.DateTime.Date == date.Value.Date);  // Assuming 'Date' is a DateTime property
        }

        if (!string.IsNullOrEmpty(category))
        {
            query = query.Where(e => e.Category == category);
        }

        if (!string.IsNullOrEmpty(location))
        {
            query = query.Where(e => e.Location == location);
        }

        var totalCount = await query.CountAsync();

        var eventEntities = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var events = _mapper.Map<ICollection<Event>>(eventEntities);

        return (events, totalCount);
    }

    
    public async Task<ICollection<User>> GetUsersAsync(Guid eventId)
    {
        var eventEntity = await _context.Events
            .AsNoTracking()
            .Include(u => u.Users)
            .FirstOrDefaultAsync(u => u.Id == eventId);
        
        var eventUsers = await _context.EventUsers
            .AsNoTracking()
            .Where(eu => eu.EventId == eventId)
            .ToListAsync();
        
        var userEntities = eventEntity.Users;
        var eventRegistrationDates = eventUsers.ToDictionary(eu => eu.UserId, eu => eu.RegistrationDate);

        var users = _mapper.Map<ICollection<User>>(userEntities);
        foreach (var user in users)
        {
            if (eventRegistrationDates.TryGetValue(user.Id, out var eventRegistrationDate))
            {
                user.RegisterDate = eventRegistrationDate;
            }
        }
        
        return users;
    }


    public async Task<Event?> GetByIdAsync(Guid id)
    {
        var eventEntity = await _context.Events
            .Include(e => e.Users)
            .AsNoTracking()
            .SingleAsync(e => e.Id == id);
        var @event = _mapper.Map<Event>(eventEntity);
        return @event;
    }

    public async Task<Event?> GetByNameAsync(string name)
    {
        var eventEntity = await _context.Events.SingleAsync(e => e.Name == name);
        var @event = _mapper.Map<Event>(eventEntity);
        return @event;
    }
    
    public async Task<Guid> AddAsync(Event @event)
    {
        var eventEntity = _mapper.Map<EventEntity>(@event);
        await _context.Events.AddAsync(eventEntity);
        return eventEntity.Id;
    }
    

    public async Task<Guid> UpdateAsync(Event @event)
    {
        var eventEntity = _mapper.Map<EventEntity>(@event);
        _context.Events.Update(eventEntity);
        return @event.Id;
    }

    
    public async Task<Guid> DeleteAsync(Guid id)
    {
        var eventEntity = await _context.Events
            .Where(e => e.Id == id)
            .SingleOrDefaultAsync();
        
        _context.Events.Remove(eventEntity);
        return id;
    }


}