using FluentResults;
using MediatR;

namespace EventsWebApplication.Application.UseCases.Events.Queries.GetEventByName;

public class GetEventByNameQuery : IRequest<Result<EventResponse>>
{
    public string Name { get; set; }
}