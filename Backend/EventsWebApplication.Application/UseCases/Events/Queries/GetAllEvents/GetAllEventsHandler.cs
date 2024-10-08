using AutoMapper;
using AutoMapper.QueryableExtensions;
using EventsWebApplication.Application.Common.Models;
using EventsWebApplication.Application.Extensions;
using EventsWebApplication.Domain.Repositories;
using FluentResults;
using MediatR;

namespace EventsWebApplication.Application.UseCases.Events.Queries.GetAllEvents;

public class GetAllEventsHandler : IRequestHandler<GetAllEventsQuery, Result<PagedResult<EventResponse>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllEventsHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<PagedResult<EventResponse>>> Handle(GetAllEventsQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.EventRepository.GetAllAsync();
        query = query
            .FilterByTitle(request.Title)
            .FilterByDate(request.Date)
            .FilterByLocation(request.Location)
            .FilterByCategory(request.Category);

        var projectedQuery = query.ProjectTo<EventResponse>(_mapper.ConfigurationProvider);
        
        return await PagedResult<EventResponse>.CreateAsync(projectedQuery, request.Page, request.PageSize, cancellationToken); 
        
    }
}