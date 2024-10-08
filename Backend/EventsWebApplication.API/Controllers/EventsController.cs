using EventsWebApplication.API.Extensions;
using EventsWebApplication.Application.UseCases.Events.Commands.AddEvent;
using EventsWebApplication.Application.UseCases.Events.Commands.DeleteEvent;
using EventsWebApplication.Application.UseCases.Events.Commands.UpdateEvent;
using EventsWebApplication.Application.UseCases.Events.Queries.GetAllEvents;
using EventsWebApplication.Application.UseCases.Events.Queries.GetEventById;
using EventsWebApplication.Application.UseCases.Events.Queries.GetEventByName;
using EventsWebApplication.Application.UseCases.Users.Queries.GetEventUsers;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EventsWebApplication.API.Controllers;

[ApiController]
[Route("[controller]")]
public class EventsController : ControllerBase
{
    private readonly ISender _sender;

    public EventsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    public async Task<IResult> GetAllEvents([FromQuery] GetAllEventsQuery request, CancellationToken cancellationToken = default)
    {
        var result = await _sender.Send(request, cancellationToken);
        return result.TryGetResult(Results.Ok);
    }
    
    [HttpGet("{eventId:guid}")]
    public async Task<IResult> GetEventById(Guid eventId, CancellationToken cancellationToken = default)
    {
        var result = await _sender.Send(new GetEventByIdQuery { EventId = eventId }, cancellationToken);
        return result.TryGetResult(Results.Ok);
    }

    [HttpGet("{name}")]
    public async Task<IResult> GetEventByName(string name, CancellationToken cancellationToken = default)
    {
        var result = await _sender.Send(new GetEventByNameQuery { Name = name }, cancellationToken);
        return result.TryGetResult(Results.Ok);
    }
    
    [HttpGet("{eventId}/users")]
    //[Authorize(Policy = "AdminOnly")]
    public async Task<IResult> GetEventUsers(Guid eventId, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var result = await _sender.Send(new GetEventUsersQuery { EventId = eventId, PageNumber = pageNumber, PageSize = pageSize }, cancellationToken);
        return result.TryGetResult(Results.Ok);
    }
    
    [HttpPost]
    [Consumes("multipart/form-data")]
    //[Authorize(Policy = "AdminOnly")]
    public async Task<IResult> AddEvent([FromForm] AddEventCommand request, CancellationToken cancellationToken = default)
    {
        var result = await _sender.Send(request, cancellationToken);
        return result.TryGetResult(value => Results.Created(string.Empty, value));
    }
    
    [HttpPut]
    [Consumes("multipart/form-data")]
    //[Authorize(Policy = "AdminOnly")]
    public async Task<IResult> UpdateEvent([FromForm] UpdateEventCommand request, CancellationToken cancellationToken = default)
    {
        var result = await _sender.Send(request, cancellationToken);
        return result.TryGetResult(Results.Ok);
    }
    
    [HttpDelete("{eventId}")]
    //[Authorize(Policy = "AdminOnly")]
    public async Task<IResult> DeleteEvent(Guid eventId, CancellationToken cancellationToken = default)
    {
        var result = await _sender.Send(new DeleteEventCommand { EventId = eventId }, cancellationToken);
        return result.TryGetResult(Results.NoContent);
    }
    
    
}


    
    