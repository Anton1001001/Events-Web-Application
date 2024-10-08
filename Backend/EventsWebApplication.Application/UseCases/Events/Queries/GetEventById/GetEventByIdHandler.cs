using AutoMapper;
using EventsWebApplication.Application.Errors;
using EventsWebApplication.Application.Helpers;
using EventsWebApplication.Domain.Repositories;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EventsWebApplication.Application.UseCases.Events.Queries.GetEventById;

public class GetEventByIdHandler : IRequestHandler<GetEventByIdQuery, Result<EventResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICookieProvider _cookieProvider;
    private readonly IJwtProvider _jwtProvider;

    public GetEventByIdHandler(IUnitOfWork unitOfWork, IMapper mapper, ICookieProvider cookieProvider, IJwtProvider jwtProvider)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _cookieProvider = cookieProvider;
        _jwtProvider = jwtProvider;
    }

    public async Task<Result<EventResponse>> Handle(GetEventByIdQuery request, CancellationToken cancellationToken)
    {
        var @event = await _unitOfWork.EventRepository.GetByIdAsync(request.EventId, cancellationToken);

        if (@event is null)
        {
            return new EventNotFoundError(message: $"Event with id: {request.EventId} not found");
        }
        
        var eventResponse = _mapper.Map<EventResponse>(@event);
        int seatsOccupied = await _unitOfWork.EventRepository
            .GetAllUsersAsync(request.EventId)
            .CountAsync(cancellationToken);
        
        eventResponse.AvailableSeats = @event.MaxUsers - seatsOccupied;
        eventResponse.IsRegistered = false;
        
        var accessToken = _cookieProvider.GetCookie("jwt");
        if (accessToken is not null)
        {
            var userId = new Guid(_jwtProvider.GetUserIdFromToken(accessToken));
            eventResponse.IsRegistered = await _unitOfWork.EventRepository
                .GetAllUsersAsync(request.EventId)
                .AnyAsync(u => u.UserId == userId, cancellationToken);
        }

        return eventResponse;
    }
}