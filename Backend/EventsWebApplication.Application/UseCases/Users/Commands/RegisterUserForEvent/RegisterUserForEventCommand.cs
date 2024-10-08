using FluentResults;
using MediatR;

namespace EventsWebApplication.Application.UseCases.Users.Commands.RegisterUserForEvent;

public class RegisterUserForEventCommand : IRequest<Result>
{
    public Guid UserId { get; set; }
    public Guid EventId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime DateOfBirth { get; set; }
}