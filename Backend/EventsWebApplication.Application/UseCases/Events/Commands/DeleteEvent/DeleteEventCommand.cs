using FluentResults;
using MediatR;

namespace EventsWebApplication.Application.UseCases.Events.Commands.DeleteEvent;

public class DeleteEventCommand : IRequest<Result>
{
    public Guid EventId { get; set; }
}