using FluentResults;
using MediatR;

namespace EventsWebApplication.Application.UseCases.Events.Queries.GetEventById;

public class GetEventByIdQuery : IRequest<Result<EventResponse>>
{
    public Guid EventId { get; set; }
}