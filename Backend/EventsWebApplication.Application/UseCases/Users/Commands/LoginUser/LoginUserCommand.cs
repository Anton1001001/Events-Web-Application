using FluentResults;
using MediatR;

namespace EventsWebApplication.Application.UseCases.Users.Commands.LoginUser;

public class LoginUserCommand : IRequest<Result<UserResponse>>
{
    public string Email { get; set; }
    public string Password { get; set; }
}