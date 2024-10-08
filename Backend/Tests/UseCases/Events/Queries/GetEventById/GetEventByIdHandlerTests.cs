using AutoMapper;
using EventsWebApplication.Application.Errors;
using EventsWebApplication.Application.Helpers;
using EventsWebApplication.Application.UseCases.Events.Queries.GetEventById;
using EventsWebApplication.Domain.Entities;
using EventsWebApplication.Domain.Repositories;
using FluentAssertions;
using MockQueryable;
using Moq;

namespace Tests.UseCases.Events.Queries.GetEventById;

public class GetEventByIdHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IEventRepository> _eventRepositoryMock = new();
    private readonly Mock<ICookieProvider> _cookieProviderMock = new();
    private readonly Mock<IJwtProvider> _jwtProviderMock = new();

    public GetEventByIdHandlerTests()
    {
        _unitOfWorkMock.Setup(u => u.EventRepository).Returns(_eventRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_EventNotFound_ReturnsEventNotFoundError()
    {
        var request = TestDataFactory.CreateGetEventByIdQuery();
        var @event = TestDataFactory.CreateEvent(request);
        var eventResponse = TestDataFactory.CreateEventResponse(@event);
        var usersQuery = TestDataFactory.CreateUsersQuery();
        
        _unitOfWorkMock.Setup(u => u.EventRepository
                .GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as Event);
        
        _mapperMock.Setup(m => m
                .Map<EventResponse>(It.IsAny<Event>()))
            .Returns(eventResponse);
        
        _unitOfWorkMock.Setup(u => u.EventRepository
                .GetAllUsersAsync(It.IsAny<Guid>()))
            .Returns(usersQuery.BuildMock());

        var handler = new GetEventByIdHandler(_unitOfWorkMock.Object, _mapperMock.Object, 
            _cookieProviderMock.Object, _jwtProviderMock.Object);
        
        var result = await handler.Handle(request, default);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainItemsAssignableTo<EventNotFoundError>();
        
        _unitOfWorkMock.Verify(u => u.EventRepository
            .GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
        
        _mapperMock.Verify(m => m
            .Map<EventResponse>(It.IsAny<Event>()), Times.Never);
        
        _unitOfWorkMock.Verify(u => u.EventRepository
            .GetAllUsersAsync(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task Handle_EventFound_ReturnsEventResponse()
    {
        var request = TestDataFactory.CreateGetEventByIdQuery();
        var @event = TestDataFactory.CreateEvent(request);
        var eventResponse = TestDataFactory.CreateEventResponse(@event);
        var usersQuery = TestDataFactory.CreateUsersQuery();
        
        _unitOfWorkMock.Setup(u => u.EventRepository
            .GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(@event);
        
        _mapperMock.Setup(m => m
            .Map<EventResponse>(It.IsAny<Event>()))
            .Returns(eventResponse);
        
        _unitOfWorkMock.Setup(u => u.EventRepository
            .GetAllUsersAsync(It.IsAny<Guid>()))
            .Returns(usersQuery.BuildMock());

        var handler = new GetEventByIdHandler(_unitOfWorkMock.Object, _mapperMock.Object, 
            _cookieProviderMock.Object, _jwtProviderMock.Object);
        
        var result = await handler.Handle(request, default);

        result.IsSuccess.Should().BeTrue();
        result.Errors.Should().BeEmpty();
        result.Value.Should().BeEquivalentTo(eventResponse);
        
        _unitOfWorkMock.Verify(u => u.EventRepository
            .GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
        
        _mapperMock.Verify(m => m
            .Map<EventResponse>(It.IsAny<Event>()), Times.Once);
        
        _unitOfWorkMock.Verify(u => u.EventRepository
            .GetAllUsersAsync(It.IsAny<Guid>()), Times.Once);
    }

}