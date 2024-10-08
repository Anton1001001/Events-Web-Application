using EventsWebApplication.Application.Common.Models;
using FluentResults;
using MediatR;

namespace EventsWebApplication.Application.UseCases.Events.Queries.GetAllEvents;

public class GetAllEventsQuery : IRequest<Result<PagedResult<EventResponse>>>
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public string? Title { get; set; }
    public DateTime? Date { get; set; }
    public string? Category { get; set; }
    public string? Location { get; set; }
}