using AutoMapper;
using AutoMapper.QueryableExtensions;
using EventsWebApplication.Application.Common.Models;
using EventsWebApplication.Application.Errors;
using EventsWebApplication.Domain.Repositories;
using FluentResults;
using MediatR;

namespace EventsWebApplication.Application.UseCases.Users.Queries.GetEventUsers;

public class GetEventUsersHandler : IRequestHandler<GetEventUsersQuery, Result<PagedResult<UserResponse>>>
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    
    public GetEventUsersHandler(IMapper mapper, IUnitOfWork unitOfWork)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }
    
    public async Task<Result<PagedResult<UserResponse>>> Handle(GetEventUsersQuery request, CancellationToken cancellationToken)
    {
        var @event = await _unitOfWork.EventRepository.GetByIdAsync(request.EventId, cancellationToken);
        
        if (@event is null)
        {
            return new EventNotFoundError(message: $"Event with id: {request.EventId} was not found");
        }
        
        var query = _unitOfWork.EventRepository.GetAllUsersAsync(request.EventId);
        var projectedQuery = query.ProjectTo<UserResponse>(_mapper.ConfigurationProvider, new {eventId = request.EventId});
        return await PagedResult<UserResponse>.CreateAsync(projectedQuery, request.PageNumber, request.PageSize, cancellationToken);
    }
}