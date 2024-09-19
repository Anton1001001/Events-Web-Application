using EventsWebApplication.Domain.Enums;

namespace EventsWebApplication.Infrastructure.DbEntities;

public class UserEntity
{
    public Guid Id { get; set; }
    public string? FirstName { get; set; } = string.Empty;
    public string? LastName { get; set; } = string.Empty;
    public DateTime? DateOfBirth { get;  set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public Role Role { get; set; }
    public ICollection<RefreshTokenEntity> RefreshTokens { get; set; } = [];
    public ICollection<EventEntity> Events { get; set; } = [];
}