namespace EventsWebApplication.Application.DTOs;

public class UserResponseDto
{
    public string? FirstName { get; set; }
    public string? LastName { get; private set; }
    public DateTime? DateOfBirth { get; private set; }
    public string Email { get; private set; }
}