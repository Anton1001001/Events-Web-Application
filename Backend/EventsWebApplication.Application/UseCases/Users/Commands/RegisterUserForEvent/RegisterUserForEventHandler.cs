using AutoMapper;
using EventsWebApplication.Application.Errors;
using EventsWebApplication.Application.Errors.Base;
using EventsWebApplication.Domain.Repositories;
using FluentResults;
using MediatR;

namespace EventsWebApplication.Application.UseCases.Users.Commands.RegisterUserForEvent;

public class RegisterUserForEventHandler : IRequestHandler<RegisterUserForEventCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public RegisterUserForEventHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result> Handle(RegisterUserForEventCommand request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.UserRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (user is null)
        {
            return new UserNotFoundError(message: $"User with id: {request.UserId} was not found");
        }
        
        var @event = await _unitOfWork.EventRepository.GetByIdAsync(request.EventId, cancellationToken);

        if (@event is null)
        {
            return new EventNotFoundError(message: $"User with id: {request.EventId} was not found");
        }
        
        _mapper.Map(request, user);
        
        await _unitOfWork.UserRepository.RegisterForEventAsync(request.UserId, request.EventId, cancellationToken);
        await _unitOfWork.UserRepository.UpdateAsync(user, cancellationToken);
        
        var success = await _unitOfWork.SaveChangesAsync(cancellationToken);

        if (!success)
        {
            return new InternalServerError("User.RegisterForEvent", "Failed to save data when update user");
        }
        
        return Result.Ok();
    }
}