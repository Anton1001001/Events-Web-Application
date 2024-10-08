using AutoMapper;
using EventsWebApplication.Application.Errors;
using EventsWebApplication.Application.Errors.Base;
using EventsWebApplication.Application.UseCases.Users.Commands.RegisterUserForEvent;
using EventsWebApplication.Domain.Entities;
using EventsWebApplication.Domain.Repositories;
using FluentAssertions;
using Moq;

namespace Tests.UseCases.Users.Commands.RegisterUserForEvent;

public class RegisterUserForEventTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly Mock<IEventRepository> _eventRepositoryMock = new();
    private readonly Mock<IMapper> _mapperMock = new();

    public RegisterUserForEventTests()
    {
        _unitOfWorkMock.Setup(u => u.UserRepository).Returns(_userRepositoryMock.Object);
        _unitOfWorkMock.Setup(u => u.EventRepository).Returns(_eventRepositoryMock.Object);
    }
    
    [Fact]
    public async Task Handle_SuccessfulRegistration_ReturnsOkResult()
    {
        // Arrange
        var request = TestDataFactory.CreateRegisterUserForEventCommand();
        var user = TestDataFactory.CreateUser(request);
        var @event = TestDataFactory.CreateEvent(request);

        _userRepositoryMock.Setup(r => r
                .GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        
        _eventRepositoryMock.Setup(r => 
                r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(@event);

        _unitOfWorkMock.Setup(u => u
                .SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var handler = new RegisterUserForEventHandler(_unitOfWorkMock.Object, _mapperMock.Object);
        
        // Act
        var result = await handler.Handle(request, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Errors.Should().BeEmpty();
        
        _userRepositoryMock.Verify(r => r
            .GetByIdAsync(It.IsAny<Guid>(), 
                It.IsAny<CancellationToken>()), Times.Once);
        
        _eventRepositoryMock.Verify(r => r
            .GetByIdAsync(It.IsAny<Guid>(), 
                It.IsAny<CancellationToken>()), Times.Once);
        
        _mapperMock.Verify(m => m
            .Map(It.IsAny<RegisterUserForEventCommand>(), 
                It.IsAny<User>()), Times.Once);

        _userRepositoryMock.Verify(r => r
            .RegisterForEventAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), 
                It.IsAny<CancellationToken>()), Times.Once);
        
        _userRepositoryMock.Verify(r => r
            .UpdateAsync(It.IsAny<User>(), 
                It.IsAny<CancellationToken>()), Times.Once);
        
        _unitOfWorkMock.Verify(u => u
            .SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_UserNotFound_ReturnsUserNotFoundError()
    {
        // Arrange
        var request = TestDataFactory.CreateRegisterUserForEventCommand();

        _userRepositoryMock.Setup(r => r
                .GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as User);

        var handler = new RegisterUserForEventHandler(_unitOfWorkMock.Object, _mapperMock.Object);
        
        // Act
        var result = await handler.Handle(request, default);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainItemsAssignableTo<UserNotFoundError>();
        
        _userRepositoryMock.Verify(r => r
            .GetByIdAsync(It.IsAny<Guid>(), 
                It.IsAny<CancellationToken>()), Times.Once);
        
        _eventRepositoryMock.Verify(r => r
            .GetByIdAsync(It.IsAny<Guid>(), 
                It.IsAny<CancellationToken>()), Times.Never);
        
        _mapperMock.Verify(m => m
            .Map(It.IsAny<RegisterUserForEventCommand>(), 
                It.IsAny<User>()), Times.Never);

        _userRepositoryMock.Verify(r => r
            .RegisterForEventAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), 
                It.IsAny<CancellationToken>()), Times.Never);
        
        _userRepositoryMock.Verify(r => r
            .UpdateAsync(It.IsAny<User>(), 
                It.IsAny<CancellationToken>()), Times.Never);
        
        _unitOfWorkMock.Verify(u => u
            .SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
    
    [Fact]
    public async Task Handle_EventNotFound_ReturnsEventNotFoundError()
    {
        // Arrange
        var request = TestDataFactory.CreateRegisterUserForEventCommand();
        var user = TestDataFactory.CreateUser(request);

        _userRepositoryMock.Setup(r => r
                .GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        
        _eventRepositoryMock.Setup(r => r
                .GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as Event);

        var handler = new RegisterUserForEventHandler(_unitOfWorkMock.Object, _mapperMock.Object);
        
        // Act
        var result = await handler.Handle(request, default);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainItemsAssignableTo<EventNotFoundError>();
        
        _userRepositoryMock.Verify(r => r
            .GetByIdAsync(It.IsAny<Guid>(), 
                It.IsAny<CancellationToken>()), Times.Once);
        
        _eventRepositoryMock.Verify(r => r
            .GetByIdAsync(It.IsAny<Guid>(), 
                It.IsAny<CancellationToken>()), Times.Once);
        
        _mapperMock.Verify(m => m
            .Map(It.IsAny<RegisterUserForEventCommand>(), 
                It.IsAny<User>()), Times.Never);

        _userRepositoryMock.Verify(r => r
            .RegisterForEventAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), 
                It.IsAny<CancellationToken>()), Times.Never);
        
        _userRepositoryMock.Verify(r => r
            .UpdateAsync(It.IsAny<User>(), 
                It.IsAny<CancellationToken>()), Times.Never);
        
        _unitOfWorkMock.Verify(u => u
            .SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_SaveChangesFails_ReturnsInternalServerError()
    {
        // Arrange
        var request = TestDataFactory.CreateRegisterUserForEventCommand();
        var user = TestDataFactory.CreateUser(request);
        var @event = TestDataFactory.CreateEvent(request);

        _userRepositoryMock.Setup(r => r
                .GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
    
        _eventRepositoryMock.Setup(r => 
                r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(@event);

        _unitOfWorkMock.Setup(u => u
                .SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var handler = new RegisterUserForEventHandler(_unitOfWorkMock.Object, _mapperMock.Object);
    
        // Act
        var result = await handler.Handle(request, default);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainItemsAssignableTo<InternalServerError>();
    
        _userRepositoryMock.Verify(r => r
            .GetByIdAsync(It.IsAny<Guid>(), 
                It.IsAny<CancellationToken>()), Times.Once);
    
        _eventRepositoryMock.Verify(r => r
            .GetByIdAsync(It.IsAny<Guid>(), 
                It.IsAny<CancellationToken>()), Times.Once);
    
        _mapperMock.Verify(m => m
            .Map(It.IsAny<RegisterUserForEventCommand>(), 
                It.IsAny<User>()), Times.Once);

        _userRepositoryMock.Verify(r => r
            .RegisterForEventAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), 
                It.IsAny<CancellationToken>()), Times.Once);
    
        _userRepositoryMock.Verify(r => r
            .UpdateAsync(It.IsAny<User>(), 
                It.IsAny<CancellationToken>()), Times.Once);
    
        _unitOfWorkMock.Verify(u => u
            .SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}