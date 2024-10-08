using EventsWebApplication.Application.Common.Models;
using FluentResults;
using MediatR;

namespace EventsWebApplication.Application.UseCases.Users.Queries.GetEventUsers;

public class GetEventUsersQuery : IRequest<Result<PagedResult<UserResponse>>>
{
    public Guid EventId { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}