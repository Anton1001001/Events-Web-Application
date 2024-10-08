using FluentResults;
using MediatR;

namespace EventsWebApplication.Application.UseCases.Users.Queries.GetUserById;

public class GetUserByIdQuery : IRequest<Result<UserResponse>>
{
    public Guid UserId { get; set; }
}