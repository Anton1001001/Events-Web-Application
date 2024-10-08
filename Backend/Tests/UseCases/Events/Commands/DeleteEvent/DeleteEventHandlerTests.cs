using EventsWebApplication.Application.Errors;
using EventsWebApplication.Application.Errors.Base;
using EventsWebApplication.Application.UseCases.Events.Commands.DeleteEvent;
using EventsWebApplication.Domain.Entities;
using EventsWebApplication.Domain.Repositories;
using FluentAssertions;
using Moq;

namespace Tests.UseCases.Events.Commands.DeleteEvent;

public class DeleteEventHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IEventRepository> _eventRepositoryMock = new();

    public DeleteEventHandlerTests()
    {
        _unitOfWorkMock.Setup(u => u.EventRepository).Returns(_eventRepositoryMock.Object);
    }
    
    [Fact]
    public async Task Handle_EventExists_DeletesEventSuccessfully()
    {
        // Arrange
        var command = TestDataFactory.CreateDeleteEventCommand();
        var @event = TestDataFactory.CreateEvent(command);

        _unitOfWorkMock.Setup(u => u.EventRepository
                .GetByIdAsync(command.EventId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(@event);
        _unitOfWorkMock.Setup(u => u
                .SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

        var handler = new DeleteEventHandler(_unitOfWorkMock.Object);

        // Act
        var result = await handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Errors.Should().BeEmpty();
        
        _unitOfWorkMock.Verify(u => u.EventRepository
            .DeleteAsync(@event, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.EventRepository
            .GetByIdAsync(@event.EventId, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u
            .SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_EventDoesNotExist_ReturnsEventNotFoundError()
    {
        // Arrange
        var command = TestDataFactory.CreateDeleteEventCommand();
        var @event = TestDataFactory.CreateEvent(command);

        _unitOfWorkMock.Setup(u => u.EventRepository
                .GetByIdAsync(command.EventId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as Event);

        var handler = new DeleteEventHandler(_unitOfWorkMock.Object);
        
        // Act
        var result = await handler.Handle(command, default);
        
        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainItemsAssignableTo<EventNotFoundError>();
        
        _unitOfWorkMock.Verify(u => u.EventRepository
            .GetByIdAsync(command.EventId, It.IsAny<CancellationToken>()), Times.Once);
        
        _unitOfWorkMock.Verify(u => u.EventRepository
            .DeleteAsync(@event, It.IsAny<CancellationToken>()), Times.Never);
        
        _unitOfWorkMock.Verify(u => u
            .SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_SaveChangesFails_ReturnsInternalServerError()
    {
        // Arrange
        var request = TestDataFactory.CreateDeleteEventCommand();
        var @event = TestDataFactory.CreateEvent(request);
        
        _unitOfWorkMock.Setup(u => u.EventRepository
                .GetByIdAsync(request.EventId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(@event);
        
        _unitOfWorkMock.Setup(u => u
                .SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var handler = new DeleteEventHandler(_unitOfWorkMock.Object);

        // Act
        var result = await handler.Handle(request, default);
        
        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainItemsAssignableTo<InternalServerError>();
        
        _unitOfWorkMock.Verify(u => u.EventRepository
            .GetByIdAsync(request.EventId, It.IsAny<CancellationToken>()), Times.Once);
        
        _unitOfWorkMock.Verify(u => u.EventRepository
            .DeleteAsync(It.IsAny<Event>(), It.IsAny<CancellationToken>()), Times.Once);
        
        _unitOfWorkMock.Verify(u => u
            .SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

}