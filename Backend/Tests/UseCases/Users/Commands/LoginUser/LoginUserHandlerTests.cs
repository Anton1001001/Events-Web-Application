using AutoMapper;
using EventsWebApplication.Application.Errors.Base;
using EventsWebApplication.Application.Helpers;
using EventsWebApplication.Application.UseCases.Users.Commands.LoginUser;
using EventsWebApplication.Domain.Entities;
using EventsWebApplication.Domain.Repositories;
using FluentAssertions;
using Moq;

namespace Tests.UseCases.Users.Commands.LoginUser;

public class LoginUserHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IJwtProvider> _jwtProviderMock = new();
    private readonly Mock<ICookieProvider> _cookieProviderMock = new();
    private readonly Mock<IUserRepository> _userRepositoryMock = new();

    public LoginUserHandlerTests()
    {
        _unitOfWorkMock.Setup(u => u.UserRepository).Returns(_userRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCredentials_ReturnsUserResponse()
    {
        // Arrange
        var request = TestDataFactory.CreateLoginUserCommand();
        var user = TestDataFactory.CreateUser(request);
        var userResponse = TestDataFactory.CreateUserResponse(user);
        var refreshToken = TestDataFactory.CreateRefreshToken();

        _userRepositoryMock.Setup(r => r
                .GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _jwtProviderMock.Setup(j => j
                .VerifyPassword(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(true);

        _jwtProviderMock.Setup(j => j
                .GenerateRefreshToken())
            .Returns(refreshToken);

        _unitOfWorkMock.Setup(u => u
                .SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);


        _mapperMock.Setup(m => m.Map<UserResponse>(It.IsAny<User>())).Returns(userResponse);

        var handler = new LoginUserHandler(
            _unitOfWorkMock.Object,
            _mapperMock.Object,
            _jwtProviderMock.Object,
            _cookieProviderMock.Object);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Errors.Should().BeEmpty();
        result.Value.Should().BeEquivalentTo(userResponse);

        _userRepositoryMock.Verify(r => r
            .GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);

        _jwtProviderMock.Verify(j => j
            .VerifyPassword(It.IsAny<string>(), It.IsAny<string>()), Times.Once);

        _jwtProviderMock.Verify(j => j
            .GenerateJwtToken(It.IsAny<User>()), Times.Once);

        _jwtProviderMock.Verify(j => j
            .GenerateRefreshToken(), Times.Once);

        _cookieProviderMock.Verify(c => c
            .SetCookie("jwt", It.IsAny<string>(), true, true), Times.Once);

        _cookieProviderMock.Verify(c => c
            .SetCookie("refresh_token", It.IsAny<string>(), true, true), Times.Once);

        _userRepositoryMock.Verify(r => r
            .RemoveExpiredRefreshTokensAsync(It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()), Times.Once);

        _userRepositoryMock.Verify(r => r
            .AddRefreshTokenAsync(It.IsAny<Guid>(), It.IsAny<RefreshToken>(),
                It.IsAny<CancellationToken>()), Times.Once);

        _unitOfWorkMock.Verify(u => u
            .SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        
        _mapperMock.Verify(m => m
            .Map<UserResponse>(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task Handle_InvalidEmail_ReturnsInvalidCredentialsError()
    {
        // Arrange
        var request = TestDataFactory.CreateLoginUserCommand();
        
        _userRepositoryMock.Setup(r => r
                .GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as User);

        var handler = new LoginUserHandler(
            _unitOfWorkMock.Object,
            _mapperMock.Object,
            _jwtProviderMock.Object,
            _cookieProviderMock.Object);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainItemsAssignableTo<InvalidCredentialsError>();

        _userRepositoryMock.Verify(r => r
            .GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);

        _jwtProviderMock.Verify(j => j
            .VerifyPassword(It.IsAny<string>(), It.IsAny<string>()), Times.Never);

        _jwtProviderMock.Verify(j => j
            .GenerateJwtToken(It.IsAny<User>()), Times.Never);

        _jwtProviderMock.Verify(j => j
            .GenerateRefreshToken(), Times.Never);
        
        _cookieProviderMock.Verify(c => c
            .SetCookie("jwt", It.IsAny<string>(), true, true), Times.Never);

        _cookieProviderMock.Verify(c => c
            .SetCookie("refresh_token", It.IsAny<string>(), true, true), Times.Never);

        _userRepositoryMock.Verify(r => r
            .RemoveExpiredRefreshTokensAsync(It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()), Times.Never);

        _userRepositoryMock.Verify(r => r
            .AddRefreshTokenAsync(It.IsAny<Guid>(), It.IsAny<RefreshToken>(),
                It.IsAny<CancellationToken>()), Times.Never);

        _unitOfWorkMock.Verify(u => u
            .SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        
        _mapperMock.Verify(m => m
            .Map<UserResponse>(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task Handle_InvalidPassword_ReturnsInvalidCredentialsError()
    {
        // Arrange
        var request = TestDataFactory.CreateLoginUserCommand();
        var user = TestDataFactory.CreateUser(request);

        _userRepositoryMock.Setup(r => r
                .GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _jwtProviderMock.Setup(j => j
                .VerifyPassword(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(false);

        var handler = new LoginUserHandler(
            _unitOfWorkMock.Object,
            _mapperMock.Object,
            _jwtProviderMock.Object,
            _cookieProviderMock.Object);

        // Act
        var result = await handler.Handle(request, default);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainItemsAssignableTo<InvalidCredentialsError>();

        _userRepositoryMock.Verify(r => r
            .GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);

        _jwtProviderMock.Verify(j => j
            .VerifyPassword(It.IsAny<string>(), It.IsAny<string>()), Times.Once);

        _jwtProviderMock.Verify(j => j
            .GenerateJwtToken(It.IsAny<User>()), Times.Never);

        _jwtProviderMock.Verify(j => j
            .GenerateRefreshToken(), Times.Never);

        _cookieProviderMock.Verify(c => c
            .SetCookie("jwt", It.IsAny<string>(), true, true), Times.Never);

        _cookieProviderMock.Verify(c => c
            .SetCookie("refresh_token", It.IsAny<string>(), true, true), Times.Never);

        _userRepositoryMock.Verify(r => r
            .RemoveExpiredRefreshTokensAsync(It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()), Times.Never);

        _userRepositoryMock.Verify(r => r
            .AddRefreshTokenAsync(It.IsAny<Guid>(), It.IsAny<RefreshToken>(),
                It.IsAny<CancellationToken>()), Times.Never);

        _unitOfWorkMock.Verify(u => u
            .SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        
        _mapperMock.Verify(m => m
            .Map<UserResponse>(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task Handle_SaveChangesFailed_ReturnsInternalServerError()
    {
        // Arrange
        var request = TestDataFactory.CreateLoginUserCommand();
        var user = TestDataFactory.CreateUser(request);
        var userResponse = TestDataFactory.CreateUserResponse(user);
        var refreshToken = TestDataFactory.CreateRefreshToken();

        _userRepositoryMock.Setup(r => r
                .GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _jwtProviderMock.Setup(j => j
                .VerifyPassword(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(true);

        _jwtProviderMock.Setup(j => j
                .GenerateRefreshToken())
            .Returns(refreshToken);

        _unitOfWorkMock.Setup(u => u
                .SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);


        _mapperMock.Setup(m => m.Map<UserResponse>(It.IsAny<User>())).Returns(userResponse);

        var handler = new LoginUserHandler(
            _unitOfWorkMock.Object,
            _mapperMock.Object,
            _jwtProviderMock.Object,
            _cookieProviderMock.Object);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainItemsAssignableTo<InternalServerError>();

        _userRepositoryMock.Verify(r => r
            .GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);

        _jwtProviderMock.Verify(j => j
            .VerifyPassword(It.IsAny<string>(), It.IsAny<string>()), Times.Once);

        _jwtProviderMock.Verify(j => j
            .GenerateJwtToken(It.IsAny<User>()), Times.Once);

        _jwtProviderMock.Verify(j => j
            .GenerateRefreshToken(), Times.Once);

        _cookieProviderMock.Verify(c => c
            .SetCookie("jwt", It.IsAny<string>(), true, true), Times.Once);

        _cookieProviderMock.Verify(c => c
            .SetCookie("refresh_token", It.IsAny<string>(), true, true), Times.Once);

        _userRepositoryMock.Verify(r => r
            .RemoveExpiredRefreshTokensAsync(It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()), Times.Once);

        _userRepositoryMock.Verify(r => r
            .AddRefreshTokenAsync(It.IsAny<Guid>(), It.IsAny<RefreshToken>(),
                It.IsAny<CancellationToken>()), Times.Once);

        _unitOfWorkMock.Verify(u => u
            .SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        
        _mapperMock.Verify(m => m
            .Map<UserResponse>(It.IsAny<User>()), Times.Never);
    }
}