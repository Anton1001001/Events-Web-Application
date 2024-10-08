namespace EventsWebApplication.Application.UseCases.Users.Queries.GetEventUsers;

public class UserResponse 
{
    public Guid UserId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public DateTime RegistrationDate { get; set; }
    public string Email { get; set; }
}