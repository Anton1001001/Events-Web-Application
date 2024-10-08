using  AutoMapper;
using AutoMapper.QueryableExtensions;
using EventsWebApplication.Application.Common.Models;
using EventsWebApplication.Application.Errors;
using EventsWebApplication.Domain.Repositories;
using FluentResults;
using MediatR;

namespace EventsWebApplication.Application.UseCases.Events.Queries.GetUserEvents;

public class GetUserEventsHandler: IRequestHandler<GetUserEventsQuery, Result<PagedResult<EventResponse>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetUserEventsHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<PagedResult<EventResponse>>> Handle(GetUserEventsQuery request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.UserRepository.GetByIdAsync(request.UserId, cancellationToken);
        
        if (user is null)
        {
            return new UserNotFoundError(message: $"User with id: {request.UserId} was not found");
        }
        
        var query = _unitOfWork.UserRepository.GetEvents(request.UserId);
        var projectedQuery = query.ProjectTo<EventResponse>(_mapper.ConfigurationProvider);
        return await PagedResult<EventResponse>.CreateAsync(projectedQuery, request.PageNumber, request.PageSize, cancellationToken);
    }
}