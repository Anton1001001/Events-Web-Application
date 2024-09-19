namespace EventsWebApplication.Application.DTOs;

public class RegisterUserForEventRequestDto
{
    public Guid UserId { get; set; }
    public Guid EventId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime DateOfBirth { get; set; }
}