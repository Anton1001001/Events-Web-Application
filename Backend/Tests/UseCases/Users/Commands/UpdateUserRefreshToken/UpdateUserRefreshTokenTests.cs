using EventsWebApplication.Application.Errors;
using EventsWebApplication.Application.Errors.Base;
using EventsWebApplication.Application.Helpers;
using EventsWebApplication.Application.UseCases.Users.Commands.UpdateUserRefreshToken;
using EventsWebApplication.Domain.Entities;
using EventsWebApplication.Domain.Repositories;
using FluentAssertions;
using Moq;

namespace Tests.UseCases.Users.Commands.UpdateUserRefreshToken;

public class UpdateUserRefreshTokenTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly Mock<IJwtProvider> _jwtProviderMock = new();
    private readonly Mock<ICookieProvider> _cookieProviderMock = new();

    public UpdateUserRefreshTokenTests()
    {
        _unitOfWorkMock.Setup(u => u.UserRepository).Returns(_userRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_SuccessfulTokenUpdate_ReturnsOkResult()
    {
        var request = TestDataFactory.CreateUpdateUserRefreshTokenCommand();
        var user = TestDataFactory.CreateUser();
        var refreshToken = TestDataFactory.CreateRefreshToken();
        
        _userRepositoryMock.Setup(r => r
            .GetByRefreshTokenAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        
        _jwtProviderMock.Setup(j => j
                .GenerateRefreshToken())
            .Returns(refreshToken);
        
        _unitOfWorkMock.Setup(u => u
            .SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var handler = new UpdateUserRefreshTokenHandler(
            _unitOfWorkMock.Object, 
            _cookieProviderMock.Object, 
            _jwtProviderMock.Object);

        var result = await handler.Handle(request, default);

        result.IsSuccess.Should().BeTrue();
        result.Errors.Should().BeEmpty();
        
        _cookieProviderMock.Verify(c => c
            .GetCookie(It.IsAny<string>()), Times.Once);
        
        _userRepositoryMock.Verify(r => r
            .GetByRefreshTokenAsync(It.IsAny<string>(), 
                It.IsAny<CancellationToken>()), Times.Once);
        
        _userRepositoryMock.Verify(u => u
            .RemoveExpiredRefreshTokensAsync(It.IsAny<Guid>(), 
                It.IsAny<CancellationToken>()), Times.Once);
        
        _userRepositoryMock.Verify(u => u
            .RemoveRefreshTokenAsync(It.IsAny<Guid>(), 
                It.IsAny<string>(), 
                It.IsAny<CancellationToken>()), Times.Once);

        _jwtProviderMock.Verify(j => j
            .GenerateJwtToken(It.IsAny<User>()), Times.Once);
        
        _jwtProviderMock.Verify(j => j
            .GenerateRefreshToken(), Times.Once);
        
        _cookieProviderMock.Verify(c => c
            .SetCookie(It.IsAny<string>(), It.IsAny<string>(), 
                true, true), Times.Exactly(2));
        
        _userRepositoryMock.Verify(u => u
            .AddRefreshTokenAsync(
                It.IsAny<Guid>(), 
                It.IsAny<RefreshToken>(), 
                It.IsAny<CancellationToken>()), Times.Once);
        
        _unitOfWorkMock.Verify(u => u
            .SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_UserNotFound_ReturnsUserNotFoundError()
    {
        var request = TestDataFactory.CreateUpdateUserRefreshTokenCommand();
        
        _userRepositoryMock.Setup(r => r
            .GetByRefreshTokenAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as User);

        var handler = new UpdateUserRefreshTokenHandler(
            _unitOfWorkMock.Object, 
            _cookieProviderMock.Object, 
            _jwtProviderMock.Object);

        var result = await handler.Handle(request, default);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainItemsAssignableTo<UserNotFoundError>();
        
        _cookieProviderMock.Verify(c => c
            .GetCookie(It.IsAny<string>()), Times.Once);
        
        _userRepositoryMock.Verify(r => r
            .GetByRefreshTokenAsync(It.IsAny<string>(), 
                It.IsAny<CancellationToken>()), Times.Once);
        
        _userRepositoryMock.Verify(u => u
            .RemoveExpiredRefreshTokensAsync(It.IsAny<Guid>(), 
                It.IsAny<CancellationToken>()), Times.Never);
        
        _userRepositoryMock.Verify(u => u
            .RemoveRefreshTokenAsync(It.IsAny<Guid>(), 
                It.IsAny<string>(), 
                It.IsAny<CancellationToken>()), Times.Never);

        _jwtProviderMock.Verify(j => j
            .GenerateJwtToken(It.IsAny<User>()), Times.Never);
        
        _jwtProviderMock.Verify(j => j
            .GenerateRefreshToken(), Times.Never);
        
        _cookieProviderMock.Verify(c => c
            .SetCookie(It.IsAny<string>(), It.IsAny<string>(), 
                true, true), Times.Never);
        
        _userRepositoryMock.Verify(u => u
            .AddRefreshTokenAsync(
                It.IsAny<Guid>(), 
                It.IsAny<RefreshToken>(), 
                It.IsAny<CancellationToken>()), Times.Never);
        
        _unitOfWorkMock.Verify(u => u
            .SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_SaveChangesFails_ReturnsInternalServerError()
    {
        var request = TestDataFactory.CreateUpdateUserRefreshTokenCommand();
        var user = TestDataFactory.CreateUser();
        var refreshToken = TestDataFactory.CreateRefreshToken();
        
        _userRepositoryMock.Setup(r => r
            .GetByRefreshTokenAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        
        _jwtProviderMock.Setup(j => j
                .GenerateRefreshToken())
            .Returns(refreshToken);
        
        _unitOfWorkMock.Setup(u => u
            .SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var handler = new UpdateUserRefreshTokenHandler(
            _unitOfWorkMock.Object, 
            _cookieProviderMock.Object, 
            _jwtProviderMock.Object);

        var result = await handler.Handle(request, default);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainItemsAssignableTo<InternalServerError>();
        
        _cookieProviderMock.Verify(c => c
            .GetCookie(It.IsAny<string>()), Times.Once);
        
        _userRepositoryMock.Verify(r => r
            .GetByRefreshTokenAsync(It.IsAny<string>(), 
                It.IsAny<CancellationToken>()), Times.Once);
        
        _userRepositoryMock.Verify(u => u
            .RemoveExpiredRefreshTokensAsync(It.IsAny<Guid>(), 
                It.IsAny<CancellationToken>()), Times.Once);
        
        _userRepositoryMock.Verify(u => u
            .RemoveRefreshTokenAsync(It.IsAny<Guid>(), 
                It.IsAny<string>(), 
                It.IsAny<CancellationToken>()), Times.Once);

        _jwtProviderMock.Verify(j => j
            .GenerateJwtToken(It.IsAny<User>()), Times.Once);
        
        _jwtProviderMock.Verify(j => j
            .GenerateRefreshToken(), Times.Once);
        
        _cookieProviderMock.Verify(c => c
            .SetCookie(It.IsAny<string>(), It.IsAny<string>(), 
                true, true), Times.Exactly(2));
        
        _userRepositoryMock.Verify(u => u
            .AddRefreshTokenAsync(
                It.IsAny<Guid>(), 
                It.IsAny<RefreshToken>(), 
                It.IsAny<CancellationToken>()), Times.Once);
        
        _unitOfWorkMock.Verify(u => u
            .SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}