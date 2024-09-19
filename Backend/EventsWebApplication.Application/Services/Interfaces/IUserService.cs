using EventsWebApplication.Application.DTOs;
using EventsWebApplication.Domain.Entities;

namespace EventsWebApplication.Application.Services.Interfaces;

public interface IUserService
{
    Task<bool> RegisterUserForEventAsync(RegisterUserForEventRequestDto request);
    Task<ICollection<Event>> GetUserEventsAsync(Guid userId); 
    Task<bool> IsUserRegisteredForEventAsync(Guid userId, Guid eventId);
    Task<User> GetUserByIdAsync(Guid id);
    Task<Guid> CancelUserRegistrationForEventAsync(Guid eventId, Guid userId);
}