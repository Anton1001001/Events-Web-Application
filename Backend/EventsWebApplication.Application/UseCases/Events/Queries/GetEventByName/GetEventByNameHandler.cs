using AutoMapper;
using EventsWebApplication.Application.Errors;
using EventsWebApplication.Application.Helpers;
using EventsWebApplication.Domain.Repositories;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EventsWebApplication.Application.UseCases.Events.Queries.GetEventByName;

public class GetEventByNameHandler : IRequestHandler<GetEventByNameQuery, Result<EventResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICookieProvider _cookieProvider;
    private readonly IJwtProvider _jwtProvider;

    public GetEventByNameHandler(IUnitOfWork unitOfWork, IMapper mapper, ICookieProvider cookieProvider, IJwtProvider jwtProvider)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _cookieProvider = cookieProvider;
        _jwtProvider = jwtProvider;
    }

    public async Task<Result<EventResponse>> Handle(GetEventByNameQuery request, CancellationToken cancellationToken)
    {
        var @event = await _unitOfWork.EventRepository.GetByNameAsync(request.Name, cancellationToken);

        if (@event is null)
        {
            return new EventNotFoundError(message: $"Event with name {request.Name} not found");
        }
        
        var eventResponse = _mapper.Map<EventResponse>(@event);
        int seatsOccupied = await _unitOfWork.EventRepository
            .GetAllUsersAsync(eventResponse.EventId)
            .CountAsync(cancellationToken);
        
        eventResponse.AvailableSeats = @event.MaxUsers - seatsOccupied;
        eventResponse.IsRegistered = false;
        
        var accessToken = _cookieProvider.GetCookie("jwt");
        if (accessToken is not null)
        {
            var userId = new Guid(_jwtProvider.GetUserIdFromToken(accessToken));
            eventResponse.IsRegistered = await _unitOfWork.EventRepository
                .GetAllUsersAsync(eventResponse.EventId)
                .AnyAsync(u => u.UserId == userId, cancellationToken);
        }

        return eventResponse;
    }
}

