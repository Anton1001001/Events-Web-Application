namespace EventsWebApplication.Application.DTOs;

public class UserViewDataDto
{
    public string? FirstName { get; private set; }
    public string? LastName { get; private set; }
    public DateTime? DateOfBirth { get; private set; }
    public DateTime RegisterDate { get; private set; }
    public string Email { get; private set; }
}