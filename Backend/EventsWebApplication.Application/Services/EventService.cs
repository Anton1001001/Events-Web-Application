using EventsWebApplication.Application.Repositories;
using EventsWebApplication.Application.Services.Interfaces;
using EventsWebApplication.Domain.Entities;

namespace EventsWebApplication.Application.Services;

public class EventService : IEventService
{
    private readonly IUnitOfWork _unitOfWork;

    public EventService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<(IEnumerable<Event>, int totalCount)> GetAllEventsAsync(
        int page,
        int pageSize,
        string? title,
        DateTime? date,
        string? category,
        string? location)
    {
        return await _unitOfWork.EventRepository.GetAllAsync(page, pageSize, title, date, category, location);
    }

    public async Task<Event?> GetEventByIdAsync(Guid id)
    {
        return await _unitOfWork.EventRepository.GetByIdAsync(id);
    }

    public async Task<Event?> GetEventByNameAsync(string name)
    {
        return await _unitOfWork.EventRepository.GetByNameAsync(name);
    }

    public async Task<Guid> AddEventAsync(Event @event)
    {
        var response = await _unitOfWork.EventRepository.AddAsync(@event);
        await _unitOfWork.SaveChangesAsync();
        return response;
    }

    public async Task<Guid> UpdateEventAsync(Event @event)
    {
        var response = await _unitOfWork.EventRepository.UpdateAsync(@event);
        await _unitOfWork.SaveChangesAsync();
        return response;
    }
    
    public async Task<ICollection<User>> GetEventUsersAsync(Guid eventId)
    {
        return await _unitOfWork.EventRepository.GetUsersAsync(eventId);
    }

    public async Task<Guid> DeleteEventAsync(Guid id)
    {
        var response = await _unitOfWork.EventRepository.DeleteAsync(id);
        await _unitOfWork.SaveChangesAsync();
        return response;
    }

}