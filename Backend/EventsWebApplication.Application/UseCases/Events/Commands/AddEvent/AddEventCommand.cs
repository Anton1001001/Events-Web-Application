using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace EventsWebApplication.Application.UseCases.Events.Commands.AddEvent;

public class AddEventCommand : IRequest<Result<EventResponse>>
{
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime DateTime { get; set; }
    public string Location { get; set; }
    public string Category { get; set; }
    public int MaxUsers { get; set; }
    public IFormFile Image { get; set; }   
}