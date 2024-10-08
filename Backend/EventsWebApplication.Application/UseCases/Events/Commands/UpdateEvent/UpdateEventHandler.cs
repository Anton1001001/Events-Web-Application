using AutoMapper;
using EventsWebApplication.Application.Errors;
using EventsWebApplication.Application.Errors.Base;
using EventsWebApplication.Application.Services;
using EventsWebApplication.Domain.Repositories;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EventsWebApplication.Application.UseCases.Events.Commands.UpdateEvent;

public class UpdateEventHandler : IRequestHandler<UpdateEventCommand, Result<EventResponse>>
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IImageService _imageService;

    public UpdateEventHandler(IMapper mapper, IUnitOfWork unitOfWork, IImageService imageService)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _imageService = imageService;
    }

    public async Task<Result<EventResponse>> Handle(UpdateEventCommand request, CancellationToken cancellationToken)
    {
        var @event = await _unitOfWork.EventRepository.GetByIdAsync(request.EventId, cancellationToken);

        if (@event is null)
        {
            return new EventNotFoundError(message: $"Event with id: {request.EventId} was not found");
        }
        
        var seatsOccupied = await _unitOfWork.EventRepository
            .GetAllUsersAsync(request.EventId)
            .CountAsync(cancellationToken);

        if (request.MaxUsers < seatsOccupied)
        {
            return new EventMaxUsersConflictError(message:
                $"Cannot update event with Id {request.EventId} because the maximum number of users " +
                $"({request.MaxUsers}) is less than the number of already occupied seats ({seatsOccupied}).");
        }
        
        var availableSeats = request.MaxUsers - seatsOccupied;
        var imageUrl = @event.ImageUrl;

        if (request.Image is not null)
        {
            imageUrl = await _imageService.SaveFileAsync(request.Image, "images");
        }

        _mapper.Map(request, @event);
        @event.ImageUrl = imageUrl;
        
        var result = await _unitOfWork.EventRepository.UpdateAsync(@event, cancellationToken);
        
        var success = await _unitOfWork.SaveChangesAsync(cancellationToken);

        if (!success)
        {
            return new InternalServerError("Event.Update", $"Failed to save data after updating " +
                                                           $"event with id: {request.EventId}");
        }
        
        var eventResponse = _mapper.Map<EventResponse>(result);
        eventResponse.AvailableSeats = availableSeats;

        return eventResponse;
    }
}