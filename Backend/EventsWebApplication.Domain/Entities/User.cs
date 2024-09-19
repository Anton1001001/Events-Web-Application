using EventsWebApplication.Domain.Enums;

namespace EventsWebApplication.Domain.Entities;
public class User
{
    public Guid Id { get; private set; }
    public string? FirstName { get; set; }
    public string? LastName { get; private set; }
    public DateTime? DateOfBirth { get; private set; }
    public DateTime? RegisterDate { get; set; }
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    public Role Role { get; private set; }

    public void SetData(string firstName, string lastName, DateTime dateOfBirth)
    {
        FirstName = firstName;
        LastName = lastName;
        DateOfBirth = dateOfBirth;
    }

    public User(Guid id, string email, string passwordHash)
    {
        Id = id;
        Email = email;
        PasswordHash = passwordHash;
    }
    
    public User(Guid id, string? firstName, string? lastName, DateTime dateOfBirth, string passwordHash, string email)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        DateOfBirth = dateOfBirth;
        Email = email;
        PasswordHash = passwordHash;
    }
}