using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace EventsWebApplication.Application.UseCases.Events.Commands.UpdateEvent;

public class UpdateEventCommand : IRequest<Result<EventResponse>>
{
    public Guid EventId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime DateTime { get; set; }
    public string Location { get; set; }
    public string Category { get; set; }
    public int MaxUsers { get; set; }
    public IFormFile? Image { get; set; }
}