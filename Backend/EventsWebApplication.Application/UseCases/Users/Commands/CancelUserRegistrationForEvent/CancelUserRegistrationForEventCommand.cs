using FluentResults;
using MediatR;

namespace EventsWebApplication.Application.UseCases.Users.Commands.CancelUserRegistrationForEvent;

public class CancelUserRegistrationForEventCommand : IRequest<Result>
{
    public Guid UserId { get; set; }
    public Guid EventId { get; set; }
}