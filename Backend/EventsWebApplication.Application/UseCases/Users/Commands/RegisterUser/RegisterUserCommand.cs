using FluentResults;
using MediatR;

namespace EventsWebApplication.Application.UseCases.Users.Commands.RegisterUser;

public class RegisterUserCommand : IRequest<Result<UserResponse>>
{
    public string Email { get; set; }
    public string Password { get; set; }
}