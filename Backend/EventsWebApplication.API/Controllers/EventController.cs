using AutoMapper;
using EventsWebApplication.Application.DTOs;
using EventsWebApplication.Application.Services.Interfaces;
using EventsWebApplication.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventsWebApplication.API.Controllers;

[ApiController]
[Route("[controller]")]
public class EventController : ControllerBase
{
    private readonly IEventService _eventService;
    private readonly IMapper _mapper;

    public EventController(IEventService eventService, IMapper mapper)
    {
        _eventService = eventService;
        _mapper = mapper;
    }

    [HttpGet]
    [HttpGet]
    public async Task<ActionResult<PagedResult<EventResponseDto>>> GetAllEvents(
        [FromQuery] int page, 
        [FromQuery] int pageSize, 
        [FromQuery] string? title = null, 
        [FromQuery] DateTime? date = null, 
        [FromQuery] string? category = null, 
        [FromQuery] string? location = null)
    {
        var (events, totalCount) = await _eventService.GetAllEventsAsync(page, pageSize, title, date, category, location);

        var response = _mapper.Map<IEnumerable<EventResponseDto>>(events);

        var pagedResult = new PagedResult<EventResponseDto>
        {
            Items = response,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };

        return Ok(pagedResult);
    }



    [HttpPost]
    [Consumes("multipart/form-data")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<Guid>> AddEvent([FromForm] EventRequestDto eventRequestDto)
    {
        var @event = _mapper.Map<Event>(eventRequestDto);
        var fileName = Guid.NewGuid() + Path.GetExtension(eventRequestDto.Image.FileName);
        var filePath = Path.Combine("wwwroot", "images", fileName);

        await using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await eventRequestDto.Image.CopyToAsync(stream);
        } 
        
        @event.AddImage("/images/" + fileName);
        
        var eventId = await _eventService.AddEventAsync(@event);
        return Ok(eventId); 
    }

    
    [HttpGet("{id}")]
    public async Task<ActionResult<EventResponseDto>> GetEventById(Guid id)
    {
        var @event = await _eventService.GetEventByIdAsync(id);
        var eventResponseDto = _mapper.Map<EventResponseDto>(@event);
        return Ok(eventResponseDto);
    }
    
    
    [HttpPut("{id}")]
    [Consumes("multipart/form-data")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<Guid>> UpdateEvent(Guid id, [FromForm] EventRequestDto eventRequestDto)
    {
        var @event = new Event(id, eventRequestDto.Name, eventRequestDto.Description, eventRequestDto.DateTime, 
            eventRequestDto.Location, eventRequestDto.Category, eventRequestDto.MaxUsers);
        if (eventRequestDto.Image != null)
        {
            var fileName = Guid.NewGuid() + Path.GetExtension(eventRequestDto.Image.FileName);
            var filePath = Path.Combine("wwwroot", "images", fileName);
            await using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await eventRequestDto.Image.CopyToAsync(stream);
            }

            @event.AddImage("/images/" + fileName);
        }
        else
        {
            @event.AddImage(eventRequestDto.ImageUrl);
        }

        return await _eventService.UpdateEventAsync(@event);
    }


    [HttpGet("{eventId}/users")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<ICollection<UserViewDataDto>>> GetEventUsers(Guid eventId)
    {
        var users = await _eventService.GetEventUsersAsync(eventId);
        var response = _mapper.Map<ICollection<UserViewDataDto>>(users);
        return Ok(response);
    }
    
    
    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<Guid>> DeleteEvent(Guid id)
    {
        return await _eventService.DeleteEventAsync(id);
    }

    
    
}