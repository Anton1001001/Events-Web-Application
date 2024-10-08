using AutoMapper;
using EventsWebApplication.Application.Errors;
using EventsWebApplication.Application.Errors.Base;
using EventsWebApplication.Application.Services;
using EventsWebApplication.Application.UseCases.Events.Commands.UpdateEvent;
using EventsWebApplication.Domain.Entities;
using EventsWebApplication.Domain.Repositories;
using FluentAssertions;
using MockQueryable;
using Moq;

namespace Tests.UseCases.Events.Commands.UpdateEvent;

public class UpdateEventHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IEventRepository> _eventRepositoryMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IImageService> _imageServiceMock = new();

    public UpdateEventHandlerTests()
    {
        _unitOfWorkMock.Setup(u => u.EventRepository).Returns(_eventRepositoryMock.Object);
    }
    
    [Fact]
    public async Task Handle_EventNotFound_ReturnsEventNotFoundError()
    {
        // Arrange
        var request = TestDataFactory.CreateUpdateEventCommand();
        var @event = TestDataFactory.CreateEvent(request);
        var eventResponse = TestDataFactory.CreateEventResponse(@event);
        
        _unitOfWorkMock.Setup(x => x.EventRepository
                .GetByIdAsync(request.EventId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as Event);
        
        _unitOfWorkMock.Setup(x => x.EventRepository
            .GetAllUsersAsync(It.IsAny<Guid>()))
            .Returns(TestDataFactory.CreateUsersQuery().BuildMock());

        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _mapperMock.Setup(m => m.Map<EventResponse>(It.IsAny<Event>())).Returns(eventResponse);

        var handler = new UpdateEventHandler(_mapperMock.Object, _unitOfWorkMock.Object, _imageServiceMock.Object);

        // Act
        var result = await handler.Handle(request, default);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainItemsAssignableTo<EventNotFoundError>();
        
        _unitOfWorkMock.Verify(x => x.EventRepository
            .GetAllUsersAsync(It.IsAny<Guid>()), Times.Never);
        
        _unitOfWorkMock.Verify(x => x.EventRepository
            .UpdateAsync(It.IsAny<Event>(), It.IsAny<CancellationToken>()), Times.Never);
        
        _unitOfWorkMock.Verify(x => x
            .SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        
        _mapperMock.Verify(m => m
            .Map<EventResponse>(It.IsAny<Event>()), Times.Never);
    }

    [Fact]
    public async Task Handle_MaxUsersLessThanOccupiedSeats_ReturnsEventMaxUsersConflictError()
    {
        // Arrange
        var request = TestDataFactory.CreateUpdateEventCommand(maxUsers: 15);
        var @event = TestDataFactory.CreateEvent(request);
        var eventResponse = TestDataFactory.CreateEventResponse(@event);
        
        _unitOfWorkMock.Setup(x => x.EventRepository
                .GetByIdAsync(request.EventId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(@event);
        
        _unitOfWorkMock.Setup(x => x.EventRepository
                .GetAllUsersAsync(It.IsAny<Guid>()))
            .Returns(TestDataFactory.CreateUsersQuery(seatsOccupied: 20).BuildMock());

        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _mapperMock.Setup(m => m.Map<EventResponse>(It.IsAny<Event>())).Returns(eventResponse);

        var handler = new UpdateEventHandler(_mapperMock.Object, _unitOfWorkMock.Object, _imageServiceMock.Object);

        // Act
        var result = await handler.Handle(request, default);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainItemsAssignableTo<EventMaxUsersConflictError>();
        
        _unitOfWorkMock.Verify(x => x.EventRepository
            .GetAllUsersAsync(It.IsAny<Guid>()), Times.Once);
        
        _unitOfWorkMock.Verify(x => x.EventRepository
            .UpdateAsync(It.IsAny<Event>(), It.IsAny<CancellationToken>()), Times.Never);
        
        _unitOfWorkMock.Verify(x => x
            .SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        
        _mapperMock.Verify(m => m
            .Map<EventResponse>(It.IsAny<Event>()), Times.Never);
    }

    [Fact]
    public async Task Handle_SaveChangesFailed_ReturnsInternalServerError()
    {
        // Arrange
        var request = TestDataFactory.CreateUpdateEventCommand();
        var @event = TestDataFactory.CreateEvent(request);
        var eventResponse = TestDataFactory.CreateEventResponse(@event);
        
        _unitOfWorkMock.Setup(x => x.EventRepository
                .GetByIdAsync(request.EventId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(@event);
        
        _unitOfWorkMock.Setup(x => x.EventRepository
                .GetAllUsersAsync(It.IsAny<Guid>()))
            .Returns(TestDataFactory.CreateUsersQuery().BuildMock());

        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _mapperMock.Setup(m => m.Map<EventResponse>(It.IsAny<Event>())).Returns(eventResponse);

        var handler = new UpdateEventHandler(_mapperMock.Object, _unitOfWorkMock.Object, _imageServiceMock.Object);

        // Act
        var result = await handler.Handle(request, default);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainItemsAssignableTo<InternalServerError>();
        
        _unitOfWorkMock.Verify(x => x.EventRepository
            .GetAllUsersAsync(It.IsAny<Guid>()), Times.Once);
        
        _unitOfWorkMock.Verify(x => x.EventRepository
            .UpdateAsync(It.IsAny<Event>(), It.IsAny<CancellationToken>()), Times.Once);
        
        _unitOfWorkMock.Verify(x => x
            .SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        
        _mapperMock.Verify(m => m
            .Map<EventResponse>(It.IsAny<Event>()), Times.Never);
    }

    [Fact]
    public async Task Handle_SuccessfulUpdate_ReturnsUpdatedEvent()
    {
        // Arrange
        var request = TestDataFactory.CreateUpdateEventCommand();
        var @event = TestDataFactory.CreateEvent(request);
        var eventResponse = TestDataFactory.CreateEventResponse(@event);
        
        _unitOfWorkMock.Setup(x => x.EventRepository
                .GetByIdAsync(request.EventId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(@event);
        
        _unitOfWorkMock.Setup(x => x.EventRepository
                .GetAllUsersAsync(It.IsAny<Guid>()))
            .Returns(TestDataFactory.CreateUsersQuery().BuildMock());

        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _mapperMock.Setup(m => m.Map<EventResponse>(It.IsAny<Event>())).Returns(eventResponse);

        var handler = new UpdateEventHandler(_mapperMock.Object, _unitOfWorkMock.Object, _imageServiceMock.Object);

        // Act
        var result = await handler.Handle(request, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Errors.Should().BeEmpty();
        result.Value.Should().BeEquivalentTo(eventResponse);
        
        _unitOfWorkMock.Verify(x => x.EventRepository
            .GetAllUsersAsync(It.IsAny<Guid>()), Times.Once);
        
        _unitOfWorkMock.Verify(x => x.EventRepository
            .UpdateAsync(It.IsAny<Event>(), It.IsAny<CancellationToken>()), Times.Once);
        
        _unitOfWorkMock.Verify(x => x
            .SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        
        _mapperMock.Verify(m => m
            .Map<EventResponse>(It.IsAny<Event>()), Times.Once);
    }

}