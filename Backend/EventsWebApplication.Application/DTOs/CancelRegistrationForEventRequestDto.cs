namespace EventsWebApplication.Application.DTOs;

public class CancelRegistrationForEventRequestDto
{
    public Guid UserId { get; set; }
    public Guid EventId { get; set; }
}