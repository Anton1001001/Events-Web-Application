using EventsWebApplication.Application.DTOs;
using EventsWebApplication.Application.Helpers;
using EventsWebApplication.Application.Repositories;
using EventsWebApplication.Application.Services;
using EventsWebApplication.Domain.Entities;
using Moq;

namespace Tests.Services;

public class AuthServiceTests
{
    private readonly AuthService _authService;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IJwtProvider> _jwtProviderMock;

    public AuthServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _jwtProviderMock = new Mock<IJwtProvider>();
        _authService = new AuthService(_jwtProviderMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task RegisterAsync_ShouldRegisterUser()
    {
        var registerDto = new RegisterDto
        {
            Email = "test@example.com",
            Password = "password123"
        };

        var userId = Guid.NewGuid();

        _unitOfWorkMock.Setup(x => x.UserRepository.AddAsync(It.IsAny<User>()))
            .ReturnsAsync(userId);

        var result = await _authService.RegisterAsync(registerDto);

        Assert.Equal(userId, result);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnAuthResponse_WhenCredentialsAreValid()
    {
        var loginDto = new LoginDto
        {
            Email = "test@example.com",
            Password = "password123"
        };

        var user = new User(Guid.NewGuid(), loginDto.Email, BCrypt.Net.BCrypt.HashPassword(loginDto.Password));

        _unitOfWorkMock.Setup(x => x.UserRepository.GetByEmailAsync(loginDto.Email))
            .ReturnsAsync(user);

        _jwtProviderMock.Setup(x => x.GenerateJwtToken(user))
            .Returns("jwt_token");

        var refreshToken = new RefreshTokenDto { Token = "refresh_token" };
        _jwtProviderMock.Setup(x => x.GenerateRefreshToken())
            .Returns(refreshToken);

        var result = await _authService.LoginAsync(loginDto);
        
        Assert.Equal("jwt_token", result.AccessToken);
        Assert.Equal("refresh_token", result.RefreshToken);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task LoginAsync_ShouldThrowException_WhenPasswordIsInvalid()
    {
        var loginDto = new LoginDto
        {
            Email = "test@example.com",
            Password = "invalid_password"
        };

        var user = new User(Guid.NewGuid(), loginDto.Email, BCrypt.Net.BCrypt.HashPassword("correct_password"));

        _unitOfWorkMock.Setup(x => x.UserRepository.GetByEmailAsync(loginDto.Email))
            .ReturnsAsync(user);

        await Assert.ThrowsAsync<Exception>(() => _authService.LoginAsync(loginDto));
    }

    [Fact]
    public async Task RefreshTokenAsync_ShouldReturnNewTokens_WhenRefreshTokenIsValid()
    {
        var user = new User(Guid.NewGuid(), "test@example.com", "password_hash");

        _unitOfWorkMock.Setup(x => x.UserRepository.GetByRefreshTokenAsync("valid_refresh_token"))
            .ReturnsAsync(user);

        _jwtProviderMock.Setup(x => x.GenerateJwtToken(user))
            .Returns("new_jwt_token");

        var newRefreshToken = new RefreshTokenDto { Token = "new_refresh_token" };
        _jwtProviderMock.Setup(x => x.GenerateRefreshToken())
            .Returns(newRefreshToken);

        var result = await _authService.RefreshTokenAsync("valid_refresh_token");

        Assert.Equal("new_jwt_token", result.AccessToken);
        Assert.Equal("new_refresh_token", result.RefreshToken);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task LogoutAsync_ShouldRemoveRefreshToken()
    {
        var user = new User(Guid.NewGuid(), "test@example.com", "password_hash");

        _unitOfWorkMock.Setup(x => x.UserRepository.GetByRefreshTokenAsync("valid_refresh_token"))
            .ReturnsAsync(user);

        await _authService.LogoutAsync("valid_refresh_token");

        _unitOfWorkMock.Verify(x => x.UserRepository.RemoveRefreshTokenAsync(user.Id, "valid_refresh_token"), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }
}