using EventsWebApplication.Application.Common.Models;
using FluentResults;
using MediatR;

namespace EventsWebApplication.Application.UseCases.Events.Queries.GetUserEvents;

public class GetUserEventsQuery : IRequest<Result<PagedResult<EventResponse>>>
{
    public Guid UserId { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}