namespace EventsWebApplication.Application.DTOs;

public class CheckRegistrationForEventDto
{
    public Guid UserId { get; set; }
    public Guid EventId { get; set; }
}