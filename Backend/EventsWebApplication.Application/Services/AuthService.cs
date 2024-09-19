using EventsWebApplication.Application.DTOs;
using EventsWebApplication.Application.Helpers;
using EventsWebApplication.Application.Repositories;
using EventsWebApplication.Application.Services.Interfaces;
using EventsWebApplication.Domain.Entities;


namespace EventsWebApplication.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtProvider _jwtProvider;
    

    public AuthService(IJwtProvider jwtProvider, IUnitOfWork unitOfWork)
    {
        _jwtProvider = jwtProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> RegisterAsync(RegisterDto registerDto)
    {
        
        if (await _unitOfWork.UserRepository.GetByEmailAsync(registerDto.Email) != null)
        {
            return Guid.Empty;
        }
        string passwordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);
        var user = new User(Guid.NewGuid(), registerDto.Email, passwordHash);
        var response = await _unitOfWork.UserRepository.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();
        return response;
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
    {
        var user = await _unitOfWork.UserRepository.GetByEmailAsync(loginDto.Email);
        if (user == null)
        {
            return null;
        }
        bool passwordValid = BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash);
        if (!passwordValid)
        {
            return null;
        }
        var jwtToken = _jwtProvider.GenerateJwtToken(user);
        var refreshTokenDto = _jwtProvider.GenerateRefreshToken();
        await _unitOfWork.UserRepository.RemoveExpiredRefreshTokensAsync(user.Id);
        await _unitOfWork.UserRepository.AddRefreshTokenAsync(user.Id, refreshTokenDto);
        await _unitOfWork.SaveChangesAsync();
        return new AuthResponseDto(jwtToken, refreshTokenDto.Token);
    }

    public async Task<AuthResponseDto> RefreshTokenAsync(string refreshToken)
    {
        var user = await _unitOfWork.UserRepository.GetByRefreshTokenAsync(refreshToken);
        
        await _unitOfWork.UserRepository.RemoveExpiredRefreshTokensAsync(user.Id);
        await _unitOfWork.UserRepository.RemoveRefreshTokenAsync(user.Id, refreshToken);
        
        var newJwtToken = _jwtProvider.GenerateJwtToken(user);
        var newRefreshToken = _jwtProvider.GenerateRefreshToken();
        
        await _unitOfWork.UserRepository.AddRefreshTokenAsync(user.Id, newRefreshToken);
        await _unitOfWork.SaveChangesAsync();

        return new AuthResponseDto(newJwtToken, newRefreshToken.Token);
    }

    public async Task LogoutAsync(string refreshToken)
    {
        var user = await _unitOfWork.UserRepository.GetByRefreshTokenAsync(refreshToken);
        await _unitOfWork.UserRepository.RemoveExpiredRefreshTokensAsync(user.Id);
        await _unitOfWork.UserRepository.RemoveRefreshTokenAsync(user.Id, refreshToken);
        await _unitOfWork.SaveChangesAsync();
    }
    
}