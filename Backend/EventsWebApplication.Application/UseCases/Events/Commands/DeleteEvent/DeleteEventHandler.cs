using EventsWebApplication.Application.Errors;
using EventsWebApplication.Application.Errors.Base;
using EventsWebApplication.Domain.Repositories;
using MediatR;
using FluentResults;

namespace EventsWebApplication.Application.UseCases.Events.Commands.DeleteEvent;

public class DeleteEventHandler : IRequestHandler<DeleteEventCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteEventHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteEventCommand request, CancellationToken cancellationToken)
    {
        var @event = await _unitOfWork.EventRepository.GetByIdAsync(request.EventId, cancellationToken);

        if (@event is null)
        {
            return new EventNotFoundError(message: $"Event with id: {request.EventId} was not found");
        }
        
        await _unitOfWork.EventRepository.DeleteAsync(@event, cancellationToken);
        
        var success = await _unitOfWork.SaveChangesAsync(cancellationToken);

        if (!success)
        {
            return new InternalServerError("Event.Delete", $"Failed to save data " +
                                                           $"after deleting event with id: {request.EventId}");
        }
        
        return Result.Ok();
    }
}