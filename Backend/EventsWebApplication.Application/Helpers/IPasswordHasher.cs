namespace EventsWebApplication.Application.Helpers;

public interface IPasswordHasher
{
    string HashPassword(string password);
}