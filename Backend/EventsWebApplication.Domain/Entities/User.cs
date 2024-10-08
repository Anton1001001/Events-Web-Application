using EventsWebApplication.Domain.Enums;

namespace EventsWebApplication.Domain.Entities;

public class User
{
    public Guid UserId { get; set; }
    public string? FirstName { get; set; } = string.Empty;
    public string? LastName { get; set; } = string.Empty;
    public DateTime? DateOfBirth { get;  set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public Role Role { get; set; }
    public ICollection<RefreshToken> RefreshTokens { get; set; } = null!;
    public ICollection<Event> Events { get; set; } = null!;
    public ICollection<EventUser> EventUsers { get; set; } = null!;
}