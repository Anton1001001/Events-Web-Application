using EventsWebApplication.API.Extensions;
using EventsWebApplication.Application.UseCases.Events.Queries.GetUserEvents;
using EventsWebApplication.Application.UseCases.Users.Commands.CancelUserRegistrationForEvent;
using EventsWebApplication.Application.UseCases.Users.Commands.LoginUser;
using EventsWebApplication.Application.UseCases.Users.Commands.LogoutUser;
using EventsWebApplication.Application.UseCases.Users.Commands.RegisterUser;
using EventsWebApplication.Application.UseCases.Users.Commands.RegisterUserForEvent;
using EventsWebApplication.Application.UseCases.Users.Commands.UpdateUserRefreshToken;
using EventsWebApplication.Application.UseCases.Users.Queries.GetUserById;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventsWebApplication.API.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private readonly ISender _sender;

    public UsersController(ISender sender)
    {
        _sender = sender;
    }
    
    [HttpGet("{userId:guid}")]
    public async Task<IResult> GetUserById(Guid userId, CancellationToken cancellationToken = default)
    {
        var result = await _sender.Send(new GetUserByIdQuery { UserId = userId }, cancellationToken);
        return result.TryGetResult(Results.Ok);
    }

    [HttpGet("{userId:guid}/events")]
    public async Task<IResult> GetUserEvents(Guid userId, int pageNumber, int pageSize,
        CancellationToken cancellationToken = default)
    {
        var result =
            await _sender.Send(
                new GetUserEventsQuery { UserId = userId, PageNumber = pageNumber, PageSize = pageSize },
                cancellationToken);
        return result.TryGetResult(Results.Ok);
    }
    
    [HttpPost("register")]
    public async Task<IResult> RegisterUser([FromBody] RegisterUserCommand request, CancellationToken cancellationToken = default)
    {
        var result = await _sender.Send(request, cancellationToken);
        return result.TryGetResult(value => Results.Created(string.Empty, value));
    }
    
    [HttpPost("login")]
    public async Task<IResult> Login([FromBody] LoginUserCommand request, CancellationToken cancellationToken = default)
    {
        var result = await _sender.Send(request, cancellationToken);
        return result.TryGetResult(Results.Ok);
    }
    
    [HttpPost("update-refresh-token")]
    public async Task<IResult> UpdateUserRefreshToken(CancellationToken cancellationToken = default)
    {
        var result = await _sender.Send(new UpdateUserRefreshTokenCommand(), cancellationToken);
        return result.TryGetResult(Results.NoContent);
    }
    
    [Authorize]
    [HttpPost("logout")]
    public async Task<IResult> LogoutUser(CancellationToken cancellationToken = default)
    {
        var result = await _sender.Send(new LogoutUserCommand(), cancellationToken);
        return result.TryGetResult(Results.NoContent);
    }
    
    [HttpPost("register-for-event")]
    public async Task<IResult> RegisterUserForEvent(RegisterUserForEventCommand request, CancellationToken cancellationToken = default)
    {
        var result = await _sender.Send(request, cancellationToken);
        return result.TryGetResult(Results.NoContent);
    }
    
    [HttpPost("cancel-registration")]
    public async Task<IResult> CancelUserRegistrationForEvent([FromBody] CancelUserRegistrationForEventCommand request, CancellationToken cancellationToken = default)
    {
        var result = await _sender.Send(request, cancellationToken);
        return result.TryGetResult(Results.NoContent);
    }
    
}

    