using EventsWebApplication.Application.Errors;
using EventsWebApplication.Application.Errors.Base;
using EventsWebApplication.Domain.Repositories;
using FluentResults;
using MediatR;


namespace EventsWebApplication.Application.UseCases.Users.Commands.CancelUserRegistrationForEvent;

public class CancelUserRegistrationForEventHandler : IRequestHandler<CancelUserRegistrationForEventCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public CancelUserRegistrationForEventHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(CancelUserRegistrationForEventCommand request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.UserRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (user is null)
        {
            return new UserNotFoundError(message: $"User with id: {request.UserId} was not found");
        }
        
        var @event = await _unitOfWork.EventRepository.GetByIdAsync(request.EventId, cancellationToken);

        if (@event is null)
        {
            return new EventNotFoundError(message: $"Event with id: {request.EventId} was not found");
        }
        
        var result = await _unitOfWork.UserRepository.CancelRegistrationForEventAsync(request.UserId, request.EventId, cancellationToken);

        if (!result)
        {
            return new EventUserNotFoundError(message: $"User with id: {request.UserId} is not " +
                                                       $"registered for event with id: {request.EventId}");
        }
        
        var success = await _unitOfWork.SaveChangesAsync(cancellationToken);

        if (!success)
        {
            return new InternalServerError("User.CancelRegistrationForEvent", $"Failed to save data when " +
                                                                              $"cancelling registration user with id: {request.UserId} " +
                                                                              $"for event with id: {request.EventId}");
        }
        
        return Result.Ok();
    }
}