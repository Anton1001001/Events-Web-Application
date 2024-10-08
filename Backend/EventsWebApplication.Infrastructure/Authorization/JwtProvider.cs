using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using EventsWebApplication.Application.Helpers;
using EventsWebApplication.Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace EventsWebApplication.Infrastructure.Authorization;


public class JwtProvider : IJwtProvider
{
    private readonly JwtOptions _jwtOptions;
    private readonly EventsWebApplicationDbContext _context;

    public JwtProvider(IOptions<JwtOptions> jwtOptions, EventsWebApplicationDbContext context)
    {
        _context = context;
        _jwtOptions = jwtOptions.Value;
    }

    public string GenerateJwtToken(User user)
    {
        var claims = new[]
        {
            new Claim(CustomClaims.UserId, user.UserId.ToString()),
            new Claim(CustomClaims.Role, user.Role.ToString()),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var expires = DateTime.UtcNow.AddMinutes(_jwtOptions.ExpiresMinutes); 

        var token = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            expires: expires,
            signingCredentials: credentials);
    
        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return tokenString;
    }
    
    public string GetUserIdFromToken(string token)
    {
        var handler = new JwtSecurityTokenHandler();

        if (!handler.CanReadToken(token))
        {
            throw new SecurityTokenException("Invalid token");
        }
        
        var jwtToken = handler.ReadJwtToken(token);
        var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == CustomClaims.UserId);
        return userIdClaim?.Value;
    }

    public bool VerifyPassword(string password, string passwordHash)
    {
        return BCrypt.Net.BCrypt.Verify(password, passwordHash);
    }


    public RefreshToken GenerateRefreshToken()
    {
        var refreshToken = new RefreshToken
        {
            Token = GetUniqueToken(),
            Expires = DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenTtl),
        };

        return refreshToken;

        string GetUniqueToken()
        {
            var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(_jwtOptions.RefreshTokenLength));
            
            var tokenIsUnique = !_context.Users.Any(p => p.RefreshTokens.Any(t => t.Token == token));

            if (!tokenIsUnique)
                return GetUniqueToken();
            return token;
        }
    }

}
