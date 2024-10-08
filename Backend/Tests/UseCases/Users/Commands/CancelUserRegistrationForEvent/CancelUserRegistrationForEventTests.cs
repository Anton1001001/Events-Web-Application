using EventsWebApplication.Application.Errors;
using EventsWebApplication.Application.Errors.Base;
using EventsWebApplication.Application.UseCases.Users.Commands.CancelUserRegistrationForEvent;
using EventsWebApplication.Domain.Entities;
using EventsWebApplication.Domain.Repositories;
using FluentAssertions;
using Moq;

namespace Tests.UseCases.Users.Commands.CancelUserRegistrationForEvent;

public class CancelUserRegistrationForEventTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IUserRepository> _userRepositoryMock = new();

    public CancelUserRegistrationForEventTests()
    {
        _unitOfWorkMock.Setup(u => u.UserRepository).Returns(_userRepositoryMock.Object);
    }
    
    [Fact]
    public async Task Handle_CancelRegistration_Successful_ReturnsOkResult()
    {
        // Arrange
        var request = TestDataFactory.CreateCancelUserRegistrationForEventCommand();
        var user = TestDataFactory.CreateUser(request);
        var @event = TestDataFactory.CreateEvent(request);

        _unitOfWorkMock.Setup(u => u.UserRepository
                .GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _unitOfWorkMock.Setup(u => u.EventRepository
                .GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(@event);

        _unitOfWorkMock.Setup(u => u.UserRepository
                .CancelRegistrationForEventAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _unitOfWorkMock.Setup(u => u
                .SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var handler = new CancelUserRegistrationForEventHandler(_unitOfWorkMock.Object);

        // Act
        var result = await handler.Handle(request, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Errors.Should().BeEmpty();

        _unitOfWorkMock.Verify(u => u.UserRepository
            .GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
        
        _unitOfWorkMock.Verify(u => u.EventRepository
            .GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
        
        _unitOfWorkMock.Verify(u => u.UserRepository
            .CancelRegistrationForEventAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), 
                It.IsAny<CancellationToken>()), Times.Once);
        
        _unitOfWorkMock.Verify(u => u
            .SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    public async Task Handle_UserNotFound_ReturnsUserNotFoundError()
    {
        // Arrange
        var request = TestDataFactory.CreateCancelUserRegistrationForEventCommand();

        _unitOfWorkMock.Setup(u => u.UserRepository.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as User);

        var handler = new CancelUserRegistrationForEventHandler(_unitOfWorkMock.Object);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainItemsAssignableTo<UserNotFoundError>();

        _unitOfWorkMock.Verify(u => u.UserRepository
            .GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
        
        _unitOfWorkMock.Verify(u => u.EventRepository
            .GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        
        _unitOfWorkMock.Verify(u => u.UserRepository
            .CancelRegistrationForEventAsync(It.IsAny<Guid>(), 
                It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        
        _unitOfWorkMock.Verify(u => u
            .SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
    
    [Fact]
    public async Task Handle_EventNotFound_ReturnsEventNotFoundError()
    {
        // Arrange
        var request = TestDataFactory.CreateCancelUserRegistrationForEventCommand();
        var user = TestDataFactory.CreateUser(request);

        _unitOfWorkMock.Setup(u => u.UserRepository
                .GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _unitOfWorkMock.Setup(u => u.EventRepository
                .GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as Event);

        var handler = new CancelUserRegistrationForEventHandler(_unitOfWorkMock.Object);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainItemsAssignableTo<EventNotFoundError>();

        _unitOfWorkMock.Verify(u => u.UserRepository
            .GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
        
        _unitOfWorkMock.Verify(u => u.EventRepository
            .GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
        
        _unitOfWorkMock.Verify(u => u.UserRepository
            .CancelRegistrationForEventAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), 
                It.IsAny<CancellationToken>()), Times.Never);
        
        _unitOfWorkMock.Verify(u => u
            .SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
    
    [Fact]
    public async Task Handle_UserNotRegisteredForEvent_ReturnsEventUserNotFoundError()
    {
        // Arrange
        var request = TestDataFactory.CreateCancelUserRegistrationForEventCommand();
        var user = TestDataFactory.CreateUser(request);
        var @event = TestDataFactory.CreateEvent(request);

        _unitOfWorkMock.Setup(u => u.UserRepository
                .GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _unitOfWorkMock.Setup(u => u.EventRepository
                .GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(@event);

        _unitOfWorkMock.Setup(u => u.UserRepository
                .CancelRegistrationForEventAsync(It.IsAny<Guid>(), 
                    It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var handler = new CancelUserRegistrationForEventHandler(_unitOfWorkMock.Object);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainItemsAssignableTo<EventUserNotFoundError>();

        _unitOfWorkMock.Verify(u => u.UserRepository
            .GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
        
        _unitOfWorkMock.Verify(u => u.EventRepository
            .GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
        
        _unitOfWorkMock.Verify(u => u.UserRepository
            .CancelRegistrationForEventAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), 
                It.IsAny<CancellationToken>()), Times.Once);
        
        _unitOfWorkMock.Verify(u => u
            .SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
    
    [Fact]
    public async Task Handle_SaveChangesFails_ReturnsInternalServerError()
    {
        // Arrange
        var request = TestDataFactory.CreateCancelUserRegistrationForEventCommand();
        var user = TestDataFactory.CreateUser(request);
        var @event = TestDataFactory.CreateEvent(request);

        _unitOfWorkMock.Setup(u => u.UserRepository
                .GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _unitOfWorkMock.Setup(u => u.EventRepository
                .GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(@event);

        _unitOfWorkMock.Setup(u => u.UserRepository
                .CancelRegistrationForEventAsync(It.IsAny<Guid>(), 
                    It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _unitOfWorkMock.Setup(u => u
                .SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var handler = new CancelUserRegistrationForEventHandler(_unitOfWorkMock.Object);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainItemsAssignableTo<InternalServerError>();

        _unitOfWorkMock.Verify(u => u.UserRepository
            .GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
        
        _unitOfWorkMock.Verify(u => u.EventRepository
            .GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
        
        _unitOfWorkMock.Verify(u => u.UserRepository
            .CancelRegistrationForEventAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), 
                It.IsAny<CancellationToken>()), Times.Once);
        
        _unitOfWorkMock.Verify(u => u
            .SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}