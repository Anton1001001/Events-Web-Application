namespace EventsWebApplication.Application.DTOs;

public class RefreshTokenDto
{
    public string Token { get; set; }
    public DateTime Expires { get; set; }
}