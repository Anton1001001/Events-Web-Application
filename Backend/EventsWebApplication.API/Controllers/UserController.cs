using AutoMapper;
using EventsWebApplication.Application.DTOs;
using EventsWebApplication.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EventsWebApplication.API.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    readonly IUserService _userService;
    readonly IMapper _mapper;

    public UserController(IUserService userService, IMapper mapper)
    {
        _userService = userService;
        _mapper = mapper;
    }

    [HttpGet("events")]
    public async Task<ICollection<EventResponseDto>> GetUserEvents(Guid userId)
    {
        var events = await _userService.GetUserEventsAsync(userId);
        var response = _mapper.Map<ICollection<EventResponseDto>>(events);
        return response;
    }
    
    

    [HttpPost("register")]
    public async Task<IActionResult> RegisterUserForEvent([FromBody] RegisterUserForEventRequestDto request)
    { 
        var result = await _userService.RegisterUserForEventAsync(request); 
        return Ok(result);
    }

    [HttpPost("cancel-registration")]
    public async Task<IActionResult> CancelRegistrationForEvent([FromBody] CancelRegistrationForEventRequestDto request)
    {
        var result = await _userService.CancelUserRegistrationForEventAsync(request.UserId, request.EventId);
        return Ok(result);
    }

    [HttpGet("check-registration")]
    public async Task<ActionResult<bool>> CheckUserRegistrationForEvent([FromQuery] Guid userId, [FromQuery] Guid eventId)
    {
        var result = await _userService.IsUserRegisteredForEventAsync(userId, eventId);
        return Ok(result);
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<EventResponseDto>> GetUserById(Guid id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        var userResponseDto = _mapper.Map<UserResponseDto>(user);
        return Ok(userResponseDto);
    }

    
}