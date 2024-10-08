using AutoMapper;
using EventsWebApplication.Application.Errors.Base;
using EventsWebApplication.Application.Services;
using EventsWebApplication.Domain.Entities;
using EventsWebApplication.Domain.Repositories;
using FluentResults;
using MediatR;

namespace EventsWebApplication.Application.UseCases.Events.Commands.AddEvent;

public class AddEventHandler : IRequestHandler<AddEventCommand, Result<EventResponse>>
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IImageService _imageService;

    public AddEventHandler(IMapper mapper, IUnitOfWork unitOfWork, IImageService imageService)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _imageService = imageService;
    }

    public async Task<Result<EventResponse>> Handle(AddEventCommand request, CancellationToken cancellationToken)
    {
        var imagePath = await _imageService.SaveFileAsync(request.Image, "images");
        var @event = _mapper.Map<Event>(request);
        @event.ImageUrl = imagePath;
        var result = await _unitOfWork.EventRepository.AddAsync(@event, cancellationToken);
        var success = await _unitOfWork.SaveChangesAsync(cancellationToken);
        if (!success)
        {
            return new InternalServerError("Event.Add","Failed to save data after adding event");
        }
        
        return _mapper.Map<EventResponse>(result);

    }
}