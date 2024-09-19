namespace EventsWebApplication.Application.Helpers;

public class JwtOptions
{
    public string SecretKey { get; set; } 
    public string Issuer { get; set; }  
    public string Audience { get; set; }  
    public int ExpiresMinutes { get; set; }
    public int RefreshTokenTtl { get; set; }
    public int RefreshTokenLength { get; set; }
}