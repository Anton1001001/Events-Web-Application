using AutoMapper;
using EventsWebApplication.Application.Errors.Base;
using EventsWebApplication.Application.Services;
using EventsWebApplication.Application.UseCases.Events.Commands.AddEvent;
using EventsWebApplication.Domain.Entities;
using EventsWebApplication.Domain.Repositories;
using FluentAssertions;
using Moq;

namespace Tests.UseCases.Events.Commands.AddEvent;

public class AddEventHandlerTests
{
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IEventRepository> _eventRepositoryMock = new();
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly Mock<IImageService> _imageServiceMock = new();

    public AddEventHandlerTests()
    {
        _unitOfWorkMock.Setup(u => u.EventRepository).Returns(_eventRepositoryMock.Object);
        _unitOfWorkMock.Setup(u => u.UserRepository).Returns(_userRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnOk_WhenEventIsSuccessfullyAdded()
    {
        // Arrange
        var request = TestDataFactory.CreateAddEventCommand();
        var @event = TestDataFactory.CreateEvent(request);
        var eventResponse = TestDataFactory.CreateEventResponse(@event);
        
        _mapperMock.Setup(x => x.Map<Event>(It.IsAny<AddEventCommand>())).Returns(@event);
        _mapperMock.Setup(x => x.Map<EventResponse>(It.IsAny<Event>())).Returns(eventResponse);
        
        _unitOfWorkMock.Setup(x =>
                x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        
        var handler = new AddEventHandler(
            _mapperMock.Object, 
            _unitOfWorkMock.Object, 
            _imageServiceMock.Object);
        
        // Act
        var result = await handler.Handle(request, default);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Errors.Should().BeEmpty();
        
        _mapperMock.Verify(
            x => x.Map<Event>(request),
            Times.Once);
        
        _mapperMock.Verify(
            x => x.Map<EventResponse>(
                It.IsAny<Event>()),
            Times.Once);
        
        _eventRepositoryMock.Verify(
            x => x.AddAsync(
                It.IsAny<Event>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnInternalServerError_WhenEventIsFailedToBeAdded()
    {
        // Arrange
        var request = TestDataFactory.CreateAddEventCommand();
        var @event = TestDataFactory.CreateEvent(request);
        var eventResponse = TestDataFactory.CreateEventResponse(@event);
        
        _mapperMock.Setup(x => x.Map<Event>(It.IsAny<AddEventCommand>())).Returns(@event);
        _mapperMock.Setup(x => x.Map<EventResponse>(It.IsAny<Event>())).Returns(eventResponse);
        
        _unitOfWorkMock.Setup(x =>
                x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        
        var handler = new AddEventHandler(
            _mapperMock.Object, 
            _unitOfWorkMock.Object, 
            _imageServiceMock.Object);
        
        // Act
        var result = await handler.Handle(request, default);
        
        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainItemsAssignableTo<InternalServerError>();
        
        _mapperMock.Verify(
            x => x.Map<Event>(request),
            Times.Once);
        
        _mapperMock.Verify(
            x => x.Map<EventResponse>(
                It.IsAny<Event>()),
            Times.Never);
        
        _eventRepositoryMock.Verify(
            x => x.AddAsync(
                It.IsAny<Event>(),
                It.IsAny<CancellationToken>()),
            Times.Once);

    }

}