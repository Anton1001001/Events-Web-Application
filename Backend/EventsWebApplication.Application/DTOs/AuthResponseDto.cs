namespace EventsWebApplication.Application.DTOs;

public class AuthResponseDto
{
    public AuthResponseDto(string accessToken, string refreshToken)
    {
        AccessToken = accessToken;
        RefreshToken = refreshToken;
    }

    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
}