namespace EventsWebApplication.Application.UseCases.Users.Commands.LoginUser;

public class UserResponse
{
    public Guid UserId { get; set; }
    public string? FirstName { get; set; } = string.Empty;
    public string? LastName { get; set; } = string.Empty;
    public DateTime? DateOfBirth { get;  set; }
    public string Email { get; set; } = string.Empty;
}