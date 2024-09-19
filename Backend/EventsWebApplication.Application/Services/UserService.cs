using EventsWebApplication.Application.DTOs;
using EventsWebApplication.Application.Repositories;
using EventsWebApplication.Application.Services.Interfaces;
using EventsWebApplication.Domain.Entities;

namespace EventsWebApplication.Application.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;

    public UserService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> RegisterUserForEventAsync(RegisterUserForEventRequestDto request)
    {
        var user = await _unitOfWork.UserRepository.GetUserByIdAsync(request.UserId);
        var exists = await _unitOfWork.UserRepository.RegisterInEventAsync(request.UserId, request.EventId);
        if (!exists)
        {
            user.SetData(request.FirstName, request.LastName, request.DateOfBirth);
            await _unitOfWork.UserRepository.UpdateAsync(user);
        }

        await _unitOfWork.SaveChangesAsync();
        return exists;
    }

    public async Task<ICollection<Event>> GetUserEventsAsync(Guid userId)
    {
        return await _unitOfWork.UserRepository.GetEventsAsync(userId);
    }

    public async Task<bool> IsUserRegisteredForEventAsync(Guid userId, Guid eventId)
    {
        return await _unitOfWork.UserRepository.IsRegisteredForEventAsync(userId, eventId);
    }

    public async Task<User> GetUserByIdAsync(Guid id)
    {
        return await _unitOfWork.UserRepository.GetUserByIdAsync(id);
    }

    public async Task<Guid> CancelUserRegistrationForEventAsync(Guid userId, Guid eventId)
    {
        var response = await _unitOfWork.UserRepository.CancelRegistrationForEventAsync(userId, eventId);
        await _unitOfWork.SaveChangesAsync();
        return response;
    }
}

