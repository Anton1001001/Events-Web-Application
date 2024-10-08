using AutoMapper;
using EventsWebApplication.Application.Errors.Base;
using EventsWebApplication.Application.Helpers;
using EventsWebApplication.Application.UseCases.Users.Commands.RegisterUser;
using EventsWebApplication.Domain.Entities;
using EventsWebApplication.Domain.Repositories;
using FluentAssertions;
using Moq;

namespace Tests.UseCases.Users.Commands.RegisterUser;

public class RegisterUserHandlerTests
{
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly Mock<IPasswordHasher> _passwordHasherMock = new();

    public RegisterUserHandlerTests()
    {
        _unitOfWorkMock.Setup(u => u.UserRepository).Returns(_userRepositoryMock.Object);
    }
    
    [Fact]
    public async Task Handle_SuccessfulRegistration_ReturnsUserResponse()
    {
        // Arrange
        var request = TestDataFactory.CreateRegisterUserCommand();
        var user = TestDataFactory.CreateUser(request);
        var userResponse = TestDataFactory.CreateUserResponse(user);

        _userRepositoryMock.Setup(r => r
                .GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as User);
        
        _userRepositoryMock.Setup(u => u
                .AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _unitOfWorkMock.Setup(u => u
                .SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        
        _mapperMock.Setup(m => m
            .Map<UserResponse>(It.IsAny<User>()))
            .Returns(userResponse);
        
        var handler = new RegisterUserHandler(_unitOfWorkMock.Object, _mapperMock.Object, _passwordHasherMock.Object);
        
        // Act
        var result = await handler.Handle(request, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Errors.Should().BeEmpty();
        result.Value.Should().BeEquivalentTo(userResponse);
        
        _userRepositoryMock.Verify(r => r
            .GetByEmailAsync(It.IsAny<string>(), 
                It.IsAny<CancellationToken>()), Times.Once);
        
        _passwordHasherMock.Verify(h => h
            .HashPassword(It.IsAny<string>()), Times.Once);
        
        _userRepositoryMock.Verify(r => r
            .AddAsync(It.IsAny<User>(), 
                It.IsAny<CancellationToken>()), Times.Once);
        
        _unitOfWorkMock.Verify(u => u
            .SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        
        _mapperMock.Verify(m => m
            .Map<UserResponse>(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task Handle_UserAlreadyExists_ReturnsUserEmailConflictError()
    {
        // Arrange
        var request = TestDataFactory.CreateRegisterUserCommand();
        var user = TestDataFactory.CreateUser(request);

        _userRepositoryMock.Setup(r => r
                .GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        
        var handler = new RegisterUserHandler(_unitOfWorkMock.Object, _mapperMock.Object, _passwordHasherMock.Object);
        
        // Act
        var result = await handler.Handle(request, default);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainItemsAssignableTo<UserEmailConflictError>();
        
        _userRepositoryMock.Verify(r => r
            .GetByEmailAsync(It.IsAny<string>(), 
                It.IsAny<CancellationToken>()), Times.Once);
        
        _passwordHasherMock.Verify(h => h
            .HashPassword(It.IsAny<string>()), Times.Never);
        
        _userRepositoryMock.Verify(r => r
            .AddAsync(It.IsAny<User>(), 
                It.IsAny<CancellationToken>()), Times.Never);
        
        _unitOfWorkMock.Verify(u => u
            .SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        
        _mapperMock.Verify(m => m
            .Map<UserResponse>(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task Handle_SaveChangesFails_ReturnsInternalServerError()
    {
        // Arrange
        var request = TestDataFactory.CreateRegisterUserCommand();
        var user = TestDataFactory.CreateUser(request);
        var userResponse = TestDataFactory.CreateUserResponse(user);

        _userRepositoryMock.Setup(r => r
                .GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as User);
        
        _userRepositoryMock.Setup(u => u
                .AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _unitOfWorkMock.Setup(u => u
                .SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        
        var handler = new RegisterUserHandler(_unitOfWorkMock.Object, _mapperMock.Object, _passwordHasherMock.Object);
        
        // Act
        var result = await handler.Handle(request, default);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainItemsAssignableTo<InternalServerError>();
        
        _userRepositoryMock.Verify(r => r
            .GetByEmailAsync(It.IsAny<string>(), 
                It.IsAny<CancellationToken>()), Times.Once);
        
        _passwordHasherMock.Verify(h => h
            .HashPassword(It.IsAny<string>()), Times.Once);
        
        _userRepositoryMock.Verify(r => r
            .AddAsync(It.IsAny<User>(), 
                It.IsAny<CancellationToken>()), Times.Once);
        
        _unitOfWorkMock.Verify(u => u
            .SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        
        _mapperMock.Verify(m => m
            .Map<UserResponse>(It.IsAny<User>()), Times.Never);
    }
}