using EventsWebApplication.Application.Errors;
using EventsWebApplication.Application.Errors.Base;
using EventsWebApplication.Application.Helpers;
using EventsWebApplication.Application.UseCases.Users.Commands.LogoutUser;
using EventsWebApplication.Domain.Entities;
using EventsWebApplication.Domain.Repositories;
using FluentAssertions;
using Moq;

namespace Tests.UseCases.Users.Commands.LogoutUser;

public class LogoutUserHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<ICookieProvider> _cookieProviderMock = new();
    private readonly Mock<IUserRepository> _userRepositoryMock = new();

    public LogoutUserHandlerTests()
    {
        _unitOfWorkMock.Setup(u => u.UserRepository).Returns(_userRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_SuccessfulLogout_ReturnsOk()
    {
        // Arrange
        var request = TestDataFactory.CreateLogoutUserCommand();
        var user = TestDataFactory.CreateUser();

        _userRepositoryMock.Setup(r => r
                .GetByRefreshTokenAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _unitOfWorkMock.Setup(u => u
                .SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        
        var handler = new LogoutUserHandler(_unitOfWorkMock.Object, _cookieProviderMock.Object);
        
        // Act
        var result = await handler.Handle(request, default);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Errors.Should().BeEmpty();
        
        _cookieProviderMock.Verify(c => c
            .GetCookie(It.IsAny<string>()), Times.Once);
        
        _cookieProviderMock.Verify(c => c
            .RemoveCookie(It.IsAny<string>()), Times.Exactly(2));
        
        _userRepositoryMock.Verify(u => u
            .GetByRefreshTokenAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        
        _userRepositoryMock.Verify(u => u
            .RemoveExpiredRefreshTokensAsync(It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()), Times.Once);
        
        _userRepositoryMock.Verify(u => u
            .RemoveRefreshTokenAsync(It.IsAny<Guid>(), It.IsAny<string>(),
                 It.IsAny<CancellationToken>()), Times.Once);
        
        _unitOfWorkMock.Verify(u => u
            .SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_UserNotFound_ReturnsUserNotFoundError()
    {
        // Arrange
        var request = TestDataFactory.CreateLogoutUserCommand();

        _userRepositoryMock.Setup(r => r
                .GetByRefreshTokenAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as User);

        var handler = new LogoutUserHandler(_unitOfWorkMock.Object, _cookieProviderMock.Object);
        
        // Act
        var result = await handler.Handle(request, default);
        
        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainItemsAssignableTo<UserNotFoundError>();
        
        _cookieProviderMock.Verify(c => c
            .GetCookie(It.IsAny<string>()), Times.Once);
        
        _cookieProviderMock.Verify(c => c
            .RemoveCookie(It.IsAny<string>()), Times.Exactly(2));
        
        _userRepositoryMock.Verify(u => u
            .GetByRefreshTokenAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        
        _userRepositoryMock.Verify(u => u
            .RemoveExpiredRefreshTokensAsync(It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()), Times.Never);
        
        _userRepositoryMock.Verify(u => u
            .RemoveRefreshTokenAsync(It.IsAny<Guid>(), It.IsAny<string>(),
                It.IsAny<CancellationToken>()), Times.Never);
        
        _unitOfWorkMock.Verify(u => u
            .SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_SaveChangesFailed_ReturnsInternalServerError()
    {
        // Arrange
        var request = TestDataFactory.CreateLogoutUserCommand();
        var user = TestDataFactory.CreateUser();

        _userRepositoryMock.Setup(r => r
                .GetByRefreshTokenAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        
        _unitOfWorkMock.Setup(u => u
                .SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var handler = new LogoutUserHandler(_unitOfWorkMock.Object, _cookieProviderMock.Object);
        
        // Act
        var result = await handler.Handle(request, default);
        
        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainItemsAssignableTo<InternalServerError>();
        
        _cookieProviderMock.Verify(c => c
            .GetCookie(It.IsAny<string>()), Times.Once);
        
        _cookieProviderMock.Verify(c => c
            .RemoveCookie(It.IsAny<string>()), Times.Exactly(2));
        
        _userRepositoryMock.Verify(u => u
            .GetByRefreshTokenAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        
        _userRepositoryMock.Verify(u => u
            .RemoveExpiredRefreshTokensAsync(It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()), Times.Once);
        
        _userRepositoryMock.Verify(u => u
            .RemoveRefreshTokenAsync(It.IsAny<Guid>(), It.IsAny<string>(),
                It.IsAny<CancellationToken>()), Times.Once);
        
        _unitOfWorkMock.Verify(u => u
            .SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}